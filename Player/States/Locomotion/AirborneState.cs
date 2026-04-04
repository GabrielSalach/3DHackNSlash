
using Godot;

[GlobalClass]
public partial class AirborneState : State
{
    [Export] private State fallState;
    [Export] private State jumpState;

    protected override State GetInitialState() => jumpState;
    protected override void SetupTransitions()
    {
        AddTransition(jumpState, fallState, () => Context.characterBody.Velocity.Y <= 0);
    }

    // protected override State GetTransition() => Context.characterBody.Velocity.Y > 0 ? jumpState : fallState;

    protected override void OnUpdatePhysics(float delta)
    {
        Machine.context.characterBody.Velocity += Machine.context.characterBody.GetGravity() * delta;
    }
}
