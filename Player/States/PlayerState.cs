
using Godot;

[GlobalClass]
public partial class PlayerState : State
{
    [Export] private GroundedState groundedState;
    [Export] private AirborneState airborneState;

    protected override State GetInitialState() => airborneState;

    protected override State GetTransition() =>
        Machine.context.characterBody.IsOnFloor() ? groundedState : airborneState;

    public override void _Ready()
    {
        groundedState.parent = this;
        airborneState.parent = this;
    }
}
