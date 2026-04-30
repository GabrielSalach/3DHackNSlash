using Godot;

[GlobalClass, Tool]
public partial class MovementState : State
{
    
    [Export] private string AnimationName = "Run";
    [Export] private float rotationSpeed;
    
    private float animationSpeed = 1.0f;
    
    [Export]
    private float Speed
    {
        get => animationSpeed * 32.0f;
        set => animationSpeed = value / 32.0f;
    }

    protected override AnimationNodeBlendTree SetupAnimationTree()
    {
        if (string.IsNullOrEmpty(AnimationName))
            return null;
        AnimationNodeBlendTree tree = new AnimationNodeBlendTree();
        AnimationNodeAnimation animation = new AnimationNodeAnimation();
        animation.Animation = AnimationName;
        tree.AddNode("Animation", animation);
        tree.ConnectNode("output", 0, "Animation");
        
        return tree;
    }

    protected override void OnUpdatePhysics(float delta)
    {
        OrientModelToVelocity(rotationSpeed * delta);
        Context.characterBody.Velocity = Context.characterBody.Basis * Context.animator.GetRootMotionPosition() / delta;
    }
    
    private void OrientModelToVelocity(float weight)
    {
        Vector3 direction = Context.MovementDirection.Normalized();
        Vector3 leftAxis = Vector3.Up.Cross(direction);
        Quaternion rotationBasis = new Basis(leftAxis, Vector3.Up, direction).GetRotationQuaternion().Normalized();
        Context.characterBody.Basis = new Basis(Context.characterBody.Transform.Basis.GetRotationQuaternion().Slerp(rotationBasis, weight));
    }
}