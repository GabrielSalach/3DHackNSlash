#if TOOLS
using Godot;

[Tool]
public partial class StateMachineViewerPlugin : EditorPlugin
{
    private StateMachineViewerDock _dock;
    private EditorSelection        _editorSelection;

    public override void _EnterTree()
    {
        _dock = new StateMachineViewerDock();
        _dock.DefaultSlot = EditorDock.DockSlot.Bottom;
        AddDock(_dock);

        _editorSelection = EditorInterface.Singleton.GetSelection();
        _editorSelection.SelectionChanged += OnSelectionChanged;
    }

    public override void _ExitTree()
    {
        if (_editorSelection != null)
            _editorSelection.SelectionChanged -= OnSelectionChanged;

        if (_dock != null)
        {
            RemoveDock(_dock);
            _dock.QueueFree();
            _dock = null;
        }
    }

    private void OnSelectionChanged()
    {
        var selectedNodes = _editorSelection.GetSelectedNodes();

        foreach (Node node in selectedNodes)
        {
            if (HasStateDriverInHierarchy(node, out Node stateDriverNode))
            {
                _dock.SetTarget(stateDriverNode);
                return;
            }
        }

        // Selection changed to a non-StateDriver node — keep dock open with last tree
    }

    private bool HasStateDriverInHierarchy(Node node, out Node found)
    {
        Node current = node;
        while (current != null)
        {
            if (current.GetScript().Obj is Script script)
            {
                string typeName = current.GetType().Name;
                if (typeName == "StateDriver" || IsStateDriverScript(script))
                {
                    found = current;
                    return true;
                }
            }
            current = current.GetParent();
        }
        found = null;
        return false;
    }

    private static bool IsStateDriverScript(Script script)
    {
        if (script == null) return false;
        return script.ResourcePath.EndsWith("StateDriver.cs");
    }
}
#endif