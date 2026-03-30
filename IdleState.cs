
using Godot;

[GlobalClass]
public partial class IdleState : State
{
    [Export]
    private CharacterBody3D character;
    
    protected override void OnEnter()
    {
        GD.Print("Entering idle state");
    }

    protected override void OnExit()
    {
        GD.Print("Exiting idle state");
    }

    protected override void OnUpdatePhysics(float delta)
    {
    }
}
