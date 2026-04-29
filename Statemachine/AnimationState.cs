
using Godot;

[GlobalClass]
public partial class AnimationState : State
{
    [Export] private string AnimationName;
    [Export] protected bool applyRootMotion;
    private AnimationNodeAnimation animation = new AnimationNodeAnimation();
    
    protected override AnimationNodeBlendTree SetupAnimationTree()
    {
        if (string.IsNullOrEmpty(AnimationName))
            return null;
        AnimationNodeBlendTree tree = new AnimationNodeBlendTree();
        animation.Animation = AnimationName;
        tree.AddNode("Animation", animation);
        tree.ConnectNode("output", 0, "Animation");
        
        return tree;
    }

    protected override void OnUpdatePhysics(float delta)
    {
        // if (applyRootMotion)
        // {
        //     ApplyRootMotion(delta, 1);
        // }
    }

    public bool IsAnimationComplete
    {
        get
        {
            float time = (float)Context.animator.GetParameter("Animation/current_position");
            float length = (float)Context.animator.GetParameter("Animation/current_length");

            return time >= length;
        }
    }
}
