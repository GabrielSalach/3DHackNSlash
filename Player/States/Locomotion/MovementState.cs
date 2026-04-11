
using Godot;

[GlobalClass]
public partial class MovementState : State
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;
    [Export] public float rotationSpeed = 5.0f;
    
    protected override void OnUpdatePhysics(float delta)
    {
        if (Context.MovementDirection != Vector3.Zero)
        {
            Vector3 velocity = Context.characterBody.Velocity;
            velocity.X = Context.MovementDirection.X * Speed;
            velocity.Z = Context.MovementDirection.Z * Speed;
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
