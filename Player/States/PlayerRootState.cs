using Godot;

[GlobalClass, Tool]
public partial class PlayerRootState : State
{
    [Export] private State groundedState;
    [Export] private State airborneState;

    protected override State GetInitialState => airborneState;

    public override void _Ready()
    {
        AddTransition(airborneState, groundedState, () => Context.characterBody.IsOnFloor());
        AddTransition(groundedState, airborneState, () => !Context.characterBody.IsOnFloor());
    }
}