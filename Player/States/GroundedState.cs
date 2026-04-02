using Godot;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private IdleState idleState;
    [Export] private MovementState groundMovement;
    [Export] private float jumpForce;
    [Export] private float friction = 5.0f;
    
    protected override State GetInitialState() => idleState;
    protected override State GetTransition() => InputHelpers.GetMovementInput().Length() > 0 ? groundMovement : idleState;

    public override void _Ready()
    {
        idleState.parent = this;
        groundMovement.parent = this;
    }

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
