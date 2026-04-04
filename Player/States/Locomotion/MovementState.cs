
using Godot;

[GlobalClass]
public partial class MovementState : State
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
    [Export] public float rotationSpeed = 5.0f;
    
    protected override State GetInitialState() => null;
    protected override void SetupTransitions() { }

    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 input = InputHelpers.GetMovementInputAsVector3();
        if (input != Vector3.Zero)
        {
            Vector3 direction = (Context.springArm.Transform.Basis * InputHelpers.GetMovementInputAsVector3()).Normalized();
            Vector3 velocity = Context.characterBody.Velocity;
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
            Context.characterBody.Velocity = velocity;
        }
        
        OrientModelToVelocity(rotationSpeed * delta);
    }

    private void OrientModelToVelocity(float weight)
    {
        Vector3 direction = Context.characterBody.Velocity.Normalized();
        Vector3 leftAxis = Vector3.Up.Cross(direction);
        Quaternion rotationBasis = new Basis(leftAxis, Vector3.Up, direction).GetRotationQuaternion().Normalized();
        Context.modelRoot.Basis = new Basis(Context.modelRoot.Transform.Basis.GetRotationQuaternion().Slerp(rotationBasis, weight));
    }
}
