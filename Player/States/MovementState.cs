
using Godot;

[GlobalClass]
public partial class MovementState : State
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;


    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 direction = (Machine.context.characterBody.Transform.Basis * InputHelpers.GetMovementInputAsVector3()).Normalized();
        Machine.context.characterBody.AddVelocity(direction * Speed);
    }
}
