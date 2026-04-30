using Godot;

[GlobalClass, Tool]
public partial class AirborneState : State
{
    [Export] private State freeFall;

    protected override State GetInitialState => freeFall;
}