using Godot;
using System;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private IdleState idleState;
    [Export] private MovementState groundMovement;
    [Export] private float jumpForce;
    [Export] private float groundFriction = 5.0f;
    
    protected override State GetInitialState() => idleState;
    protected override State GetTransition() => InputHelpers.GetMovementInput().Length() > 0 ? groundMovement : idleState;

    public override void _Ready()
    {
        idleState.parent = this;
        groundMovement.parent = this;
    }

    protected override void OnUpdatePhysics(float delta)
    {
        if (Input.IsActionJustPressed("jump"))
        {
            Context.characterBody.AddVelocity(new Vector3(0, jumpForce, 0));
        }
        Context.characterBody.ApplyFriction(groundFriction);
    }

}
