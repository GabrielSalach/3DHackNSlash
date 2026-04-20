#if TOOLS
using Godot;
using System.Collections.Generic;

[Tool]
public partial class TreeView : Control
{
    // ── Layout ───────────────────────────────────────────────────────────────
    private const float PadH   = 6f;
    private const float PadV   = 5f;
    private const float HGap   = 16f;
    private const float VGap   = 52f;
    private const float CanPad = 20f;

    // ── Colours ──────────────────────────────────────────────────────────────
    private static readonly Color CCardBg      = new Color("#2e333d");
    private static readonly Color CCardBorder  = new Color("#555e6e");
    private static readonly Color CLeafBorder  = new Color("#4aaa66");
    private static readonly Color CText        = new Color("#dce8f5");
    private static readonly Color CConnector   = new Color("#6677aa");

    // Active path highlight colours
    private static readonly Color CActiveBg     = new Color("#1a3a5c");
    private static readonly Color CActiveBorder = new Color("#4da6ff");
    private static readonly Color CActiveText   = new Color("#ffffff");
    private static readonly Color CActiveConn   = new Color("#4da6ff");
    // Leaf of the active path (the truly current state)
    private static readonly Color CLeafActiveBg     = new Color("#0d3d1f");
    private static readonly Color CLeafActiveBorder = new Color("#44ff88");
    private static readonly Color CLeafActiveText   = new Color("#aaffcc");

    // ── Data ─────────────────────────────────────────────────────────────────
    private class CardData
    {
        public Node           Node;
        public bool           IsLeaf;
        public Rect2          Rect;
        public int            Level;
        private string        _label;
        public string         Label => _label ??= ComputeLabel(Node);
        public CardData       Parent;
        public List<CardData> Children     = new();
        public float          SubtreeWidth;

        private static string ComputeLabel(Node n)
        {
            string t = n.GetType().Name;
            return (t == "Node" || t == "CharacterBody3D") ? n.Name : t;
        }
    }

    private readonly List<CardData>  _all        = new();
    private          CardData         _root;
    private          HashSet<NodePath> _activePath = new();

    // ── Public API ───────────────────────────────────────────────────────────

    public override void _Ready()
    {
        SizeFlagsHorizontal = SizeFlags.Fill;
        SizeFlagsVertical   = SizeFlags.Fill;
        MouseFilter         = MouseFilterEnum.Ignore;
    }

    public void ClearAllNodes()
    {
        _all.Clear();
        _root       = null;
        _activePath = new HashSet<NodePath>();
        CustomMinimumSize = Vector2.Zero;
        QueueRedraw();
    }

    public void SetActivePath(HashSet<NodePath> activePath)
    {
        _activePath = activePath;
        QueueRedraw();
    }

    public void Build(Node rootNode)
    {
        _all.Clear();
        _root       = null;
        _activePath = new HashSet<NodePath>();

        Font font = GetSafeFont();
        int  fs   = GetSafeFontSize();

        _root = BuildCard(rootNode, null, 0, font, fs);
        ComputeSubtreeWidths(_root);
        AssignPositions(_root, CanPad, CanPad + _root.SubtreeWidth);

        float maxX = 0f, maxY = 0f;
        foreach (var c in _all)
        {
            maxX = Mathf.Max(maxX, c.Rect.End.X);
            maxY = Mathf.Max(maxY, c.Rect.End.Y);
        }
        CustomMinimumSize = new Vector2(maxX + CanPad, maxY + CanPad);
        QueueRedraw();
    }

    // ── Build ─────────────────────────────────────────────────────────────────

    private CardData BuildCard(Node node, CardData parent, int level, Font font, int fs)
    {
        var card = new CardData { Node = node, Level = level, Parent = parent };
        _all.Add(card);

        Vector2 textSize = font.GetStringSize(card.Label, HorizontalAlignment.Left, -1, fs);
        card.Rect = new Rect2(0, 0, textSize.X + PadH * 2f, textSize.Y + PadV * 2f);

        foreach (Node child in node.GetChildren())
            if (IsStateNode(child))
                card.Children.Add(BuildCard(child, card, level + 1, font, fs));

        card.IsLeaf = card.Children.Count == 0;
        return card;
    }

    // ── Layout ───────────────────────────────────────────────────────────────

    private void ComputeSubtreeWidths(CardData card)
    {
        if (card.IsLeaf) { card.SubtreeWidth = card.Rect.Size.X; return; }
        foreach (var child in card.Children) ComputeSubtreeWidths(child);
        float total = HGap * (card.Children.Count - 1);
        foreach (var child in card.Children) total += child.SubtreeWidth;
        card.SubtreeWidth = Mathf.Max(total, card.Rect.Size.X);
    }

    private void AssignPositions(CardData card, float left, float right)
    {
        float w  = card.Rect.Size.X;
        float h  = card.Rect.Size.Y;
        float cx = (left + right) * 0.5f - w * 0.5f;
        float cy = CanPad + card.Level * (VGap + MaxCardHeight());
        card.Rect = new Rect2(cx, cy, w, h);
        if (card.IsLeaf) return;
        float cursor = left;
        foreach (var child in card.Children)
        {
            AssignPositions(child, cursor, cursor + child.SubtreeWidth);
            cursor += child.SubtreeWidth + HGap;
        }
    }

    private float MaxCardHeight()
    {
        float max = 0f;
        foreach (var c in _all) max = Mathf.Max(max, c.Rect.Size.Y);
        return max;
    }

    // ── Draw ──────────────────────────────────────────────────────────────────

    public override void _Draw()
    {
        if (_root == null) return;

        Font font = GetSafeFont();
        int  fs   = GetSafeFontSize();

        // Connectors
        foreach (var card in _all)
        {
            if (card.Parent == null) continue;

            bool connActive = _activePath.Contains(card.Node.GetPath()) &&
                              _activePath.Contains(card.Parent.Node.GetPath());
            Color connColor = connActive ? CActiveConn : CConnector;
            float connWidth = connActive ? 2f : 1f;

            Vector2 from = new Vector2(card.Parent.Rect.Position.X + card.Parent.Rect.Size.X * 0.5f,
                                       card.Parent.Rect.End.Y);
            Vector2 to   = new Vector2(card.Rect.Position.X + card.Rect.Size.X * 0.5f,
                                       card.Rect.Position.Y);
            DrawLine(from, to, connColor, connWidth, true);
        }

        // Cards
        foreach (var card in _all)
        {
            bool isActive     = _activePath.Contains(card.Node.GetPath());
            bool isActiveLeaf = isActive && IsActiveLeaf(card);

            Color bg, border, textColor;
            if (isActiveLeaf)
            {
                bg        = CLeafActiveBg;
                border    = CLeafActiveBorder;
                textColor = CLeafActiveText;
            }
            else if (isActive)
            {
                bg        = CActiveBg;
                border    = CActiveBorder;
                textColor = CActiveText;
            }
            else
            {
                bg        = CCardBg;
                border    = card.IsLeaf ? CLeafBorder : CCardBorder;
                textColor = CText;
            }

            DrawRect(card.Rect, bg);
            DrawRect(card.Rect, border, false, isActive ? 2f : 1f);

            float textX = card.Rect.Position.X + PadH;
            float textY = card.Rect.Position.Y + card.Rect.Size.Y - PadV;
            DrawString(font, new Vector2(textX, textY), card.Label,
                       HorizontalAlignment.Left, -1, fs, textColor);
        }
    }

    // A card is the active leaf if it's in the active path but none of its children are
    private bool IsActiveLeaf(CardData card)
    {
        foreach (var child in card.Children)
            if (_activePath.Contains(child.Node.GetPath())) return false;
        return true;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private Font GetSafeFont()
    {
        Font f = GetThemeFont("font", "Label");
        return f ?? ThemeDB.FallbackFont;
    }

    private int GetSafeFontSize()
    {
        int s = GetThemeFontSize("font_size", "Label");
        return s > 0 ? s : 13;
    }

    private static bool IsStateNode(Node node)
    {
        System.Type t = node.GetType();
        while (t != null && t != typeof(object))
        {
            if (t.Name == "State") return true;
            t = t.BaseType;
        }
        if (node.GetScript().Obj is Script script)
        {
            if (IsStatePath(script.ResourcePath)) return true;
            Resource bs = script.Get("base_script").AsGodotObject() as Resource;
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
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        return name == "State" || name.EndsWith("State") || name.StartsWith("State");
    }
}
#endif