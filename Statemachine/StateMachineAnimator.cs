using Godot;

[GlobalClass]
public partial class StateMachineAnimator : AnimationTree
{
    [Export] private float defaultStateBlendTime = 0.1f;
    
    private string activeTreeName;
    private const string treeA = "TreeA";
    private const string treeB = "TreeB";
    private const string mainBlend = "MainBlend";
    
    private AnimationNodeBlendTree root = new AnimationNodeBlendTree();
    
    
    public override void _Ready()
    {
        TreeRoot = root;
        
        root.AddNode(mainBlend, new AnimationNodeBlend2());
        root.ConnectNode("output", 0, mainBlend);
    }

    public void ConnectTree(AnimationNodeBlendTree tree)
    {
        if (string.IsNullOrEmpty(activeTreeName))
        {
            root.AddNode(treeA, tree);
            root.ConnectNode(mainBlend, 0, treeA);
            root.AddNode(treeB, tree);
            root.ConnectNode(mainBlend, 1, treeB);
            activeTreeName = treeA;
            return;
        }

        if (activeTreeName == treeA)
        {
            if (root.HasNode(treeB))
            {
                root.DisconnectNode(mainBlend, 1);
                root.RemoveNode(treeB);
            }
            root.AddNode(treeB, tree);
            root.ConnectNode(mainBlend, 1, treeB);
            activeTreeName = treeB;
            // Set("parameters/MainBlend/blend_amount", 1);
            Tween blendTween = GetTree().CreateTween();
            blendTween.TweenProperty(this, "parameters/MainBlend/blend_amount", 1, 0.1f);
        }
        else
        {
            if (root.HasNode(treeA))
            {
                root.DisconnectNode(mainBlend, 0);
                root.RemoveNode(treeA);
            }
            root.AddNode(treeA, tree);
            root.ConnectNode(mainBlend, 0, treeA);
            activeTreeName = treeA;
            // Set("parameters/MainBlend/blend_amount", 0);
            Tween blendTween = GetTree().CreateTween();
            blendTween.TweenProperty(this, "parameters/MainBlend/blend_amount", 0, 0.1f);
        }
    }

    public void SetParameter(string parameter, Variant value)
    {
        Set($"parameters/{activeTreeName}/{parameter}", value);
    }

    public Variant GetParameter(string parameter)
    {
        return Get($"parameters/{activeTreeName}/{parameter}");
    }
}
