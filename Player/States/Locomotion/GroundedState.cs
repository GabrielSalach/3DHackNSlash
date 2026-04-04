using Godot;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private State idleState;
    [Export] private MovementState groundMovement;
    
    [Export] private float jumpForce;
    [Export] private float friction = 5.0f;
    
    protected override State GetInitialState() => idleState;
    protected override void SetupTransitions()
    {
        AddTransition(groundMovement, idleState, () => InputHelpers.GetMovementInput().Length() <= 0);
        AddTransition(idleState, groundMovement, () => InputHelpers.GetMovementInput().Length() > 0);
    }

    // protected override State GetTransition()
    // {
    //     return InputHelpers.GetMovementInput().Length() > 0 ? groundMovement : idleState;
    // }

    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 vel = Context.characterBody.Velocity;
        if (Input.IsActionJustPressed("jump"))
        {
            vel.Y = jumpForce;
        }
        vel.X = Mathf.MoveToward(vel.X, 0, friction * delta);
        vel.Z = Mathf.MoveToward(vel.Z, 0, friction * delta);
        Context.characterBody.Velocity = vel;
    }
}
