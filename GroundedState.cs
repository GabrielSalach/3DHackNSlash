using Godot;
using System;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private IdleState idleState;
    [Export] private MovementState groundMovement;
    [Export] private CharacterBody3D character;
    [Export] private float Friction;
    
    protected override State GetInitialState() => idleState;
    protected override State GetTransition() => machine.context.input.Length() > 0 ? groundMovement : idleState;

    public override void _Ready()
    {
        idleState.parent = this;
        groundMovement.parent = this;
    }

    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 velocity = character.Velocity;
        
        // Add the gravity.
        if (!character.IsOnFloor())
        {
            velocity += character.GetGravity() * delta;
        }
        
        if (machine.context.input == Vector2.Zero)
        {
            velocity.X = Mathf.MoveToward(character.Velocity.X, 0, Friction);
            velocity.Z = Mathf.MoveToward(character.Velocity.Z, 0, Friction);
        }
        
        character.Velocity = velocity;
        character.MoveAndSlide();
        
        character.Velocity = velocity;
        character.MoveAndSlide();
    }
}
