
using Godot;

[GlobalClass]
public partial class AirborneState : State
{
    [Export] private FallState fallState;
    [Export] private JumpState jumpState;

    protected override State GetInitialState() => jumpState;
    protected override State GetTransition() => Context.characterBody.Velocity.Y > 0 ? jumpState : fallState;

    protected override void OnUpdatePhysics(float delta)
    {
        Machine.context.characterBody.Velocity += Machine.context.characterBody.GetGravity() * delta;
    }
}
