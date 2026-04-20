#if TOOLS
using Godot;
using System.Collections.Generic;

[Tool]
public partial class StateMachineViewerDock : EditorDock
{
    private Node            _target;
    private Node            _rootStateNode;
    private NodePath        _rootStatePath;
    private TreeView        _treeView;
    private Label           _emptyLabel;
    private Label           _headerLabel;
    private ScrollContainer _scroll;

    private const float TickInterval = 0.05f;
    private float       _tickTimer   = 0f;

    public override void _Ready()
    {
        // EditorDock manages its own sizing — just use a plain VBoxContainer
        var root = new VBoxContainer();
        root.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        root.SizeFlagsVertical   = SizeFlags.ExpandFill;
        AddChild(root);

        // ── Header ───────────────────────────────────────────────────────────
        var header = new HBoxContainer();
        header.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        header.AddThemeConstantOverride("separation", 8);
        root.AddChild(header);

        _headerLabel = new Label();
        _headerLabel.Text                = "State Machine Viewer";
        _headerLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _headerLabel.VerticalAlignment   = VerticalAlignment.Center;
        header.AddChild(_headerLabel);

        var refreshButton = new Button();
        refreshButton.Text        = "⟳ Refresh";
        refreshButton.TooltipText = "Rebuild the state tree";
        refreshButton.Pressed    += () => BuildTree(_target);
        header.AddChild(refreshButton);

        root.AddChild(new HSeparator());

        // ── Body: empty label OR scroll+tree, both ExpandFill ─────────────────
        // We swap visibility between the two; they both live in the same slot.
        // Wrap in a single container so VBox gives all remaining space to it.
        var body = new MarginContainer();
        body.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        body.SizeFlagsVertical   = SizeFlags.ExpandFill;
        root.AddChild(body);

        _emptyLabel = new Label();
        _emptyLabel.Text                = "Select a node with a StateDriver script\nto visualise its state machine.";
        _emptyLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _emptyLabel.VerticalAlignment   = VerticalAlignment.Center;
        _emptyLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _emptyLabel.SizeFlagsVertical   = SizeFlags.ExpandFill;
        _emptyLabel.AutowrapMode        = TextServer.AutowrapMode.Word;
        body.AddChild(_emptyLabel);

        _scroll = new ScrollContainer();
        _scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _scroll.SizeFlagsVertical   = SizeFlags.ExpandFill;
        _scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Auto;
        _scroll.VerticalScrollMode   = ScrollContainer.ScrollMode.Auto;
        _scroll.Visible              = false;
        body.AddChild(_scroll);

        _treeView = new TreeView();
        _treeView.SizeFlagsHorizontal = SizeFlags.Fill;
        _treeView.SizeFlagsVertical   = SizeFlags.Fill;
        _scroll.AddChild(_treeView);
    }

    // ── Runtime polling ───────────────────────────────────────────────────────

    public override void _Process(double delta)
    {
        if (_rootStatePath == default || _rootStatePath.IsEmpty) return;
        if (!EditorInterface.Singleton.IsPlayingScene())
        {
            // Back from play mode: clear highlights
            _treeView.SetActivePath(new HashSet<NodePath>());
            return;
        }

        _tickTimer += (float)delta;
        if (_tickTimer < TickInterval) return;
        _tickTimer = 0f;

        // During play, editor nodes are frozen — resolve the live node via path from tree root
        Node liveRoot = GetTree().Root.GetNodeOrNull(_rootStatePath);
        if (liveRoot == null) return;

        _treeView.SetActivePath(CollectActivePath(liveRoot));
    }

    private static readonly System.Reflection.FieldInfo ActiveStateField =
        typeof(State).GetField("activeState",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

    private static HashSet<NodePath> CollectActivePath(Node rootState)
    {
        var set      = new HashSet<NodePath>();
        Node current = rootState;
        while (current is State s)
        {
            // Store the path relative to tree root so it matches editor-side nodes
            set.Add(current.GetPath());
            var next = ActiveStateField?.GetValue(s) as State;
            if (next != null)
                current = next;
            else
                break;
        }
        return set;
    }

    // ── Public API ───────────────────────────────────────────────────────────

    public void SetTarget(Node stateDriver)
    {
        _target = stateDriver;
        BuildTree(stateDriver);
    }

    public void ClearTree()
    {
        _target        = null;
        _rootStateNode = null;
        _treeView.ClearAllNodes();
        _scroll.Visible      = false;
        _emptyLabel.Visible  = true;
        _headerLabel.Text    = "State Machine Viewer";
    }

    // ── Tree build ───────────────────────────────────────────────────────────

    private void BuildTree(Node stateDriver)
    {
        if (stateDriver == null) { ClearTree(); return; }

        _headerLabel.Text = $"State Machine  —  {stateDriver.Name}";
        _rootStateNode = FindRootState(stateDriver);
        _rootStatePath = _rootStateNode != null ? _rootStateNode.GetPath() : default;

        if (_rootStateNode == null)
        {
            _treeView.ClearAllNodes();
            _emptyLabel.Text    = "No root State node found in StateDriver.";
            _emptyLabel.Visible = true;
            _scroll.Visible     = false;
            return;
        }

        _emptyLabel.Visible = false;
        _scroll.Visible     = true;
        _treeView.Build(_rootStateNode);
    }

    // ── Root-state detection ─────────────────────────────────────────────────

    private Node FindRootState(Node stateDriver)
    {
        // 1) Exported property "root_state"
        var v = stateDriver.Get("root_state");
        if (v.VariantType == Variant.Type.Object &&
            v.AsGodotObject() is Node expNode && expNode != null)
            return expNode;
        if (v.VariantType == Variant.Type.NodePath)
        {
            var np = v.AsNodePath();
            if (np != null && !np.IsEmpty)
            {
                var r = stateDriver.GetNodeOrNull(np);
                if (r != null) return r;
            }
        }
        // 2) Direct children
        foreach (Node child in stateDriver.GetChildren())
            if (IsStateNode(child)) return child;
        // 3) Recursive
        foreach (Node child in stateDriver.GetChildren())
        {
            var f = FindDeep(child);
            if (f != null) return f;
        }
        return null;
    }

    private static Node FindDeep(Node parent)
    {
        foreach (Node child in parent.GetChildren())
            if (IsStateNode(child)) return child;
        foreach (Node child in parent.GetChildren())
        {
            var f = FindDeep(child);
            if (f != null) return f;
        }
        return null;
    }

    internal static bool IsStateNode(Node node)
    {
        for (var t = node.GetType(); t != null && t != typeof(object); t = t.BaseType)
            if (t.Name == "State") return true;

        if (node.GetScript().Obj is Script script)
        {
            if (IsStatePath(script.ResourcePath)) return true;
            var bs = script.Get("base_script").AsGodotObject() as Resource;
            while (bs != null)
            {
                if (IsStatePath(bs.ResourcePath)) return true;
                bs = bs.Get("base_script").AsGodotObject() as Resource;
            }
        }
        return false;
    }

    private static bool IsStatePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        var name = System.IO.Path.GetFileNameWithoutExtension(path);
        return name == "State" || name.EndsWith("State") || name.StartsWith("State");
    }
}
#endif