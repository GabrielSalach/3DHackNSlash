using Godot;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private IdleState idleState;
    [Export] private MovementState groundMovement;
    [Export] private SwordState swordState;
    
    [Export] private float jumpForce;
    [Export] private float friction = 5.0f;
    
    protected override State GetInitialState() => idleState;

    protected override State GetTransition()
    {
        if (Input.IsActionJustPressed("light_attack"))
        {
            return swordState;
        }

        if (Input.IsActionJustPressed("heavy_attack"))
        {
            return swordState;
        }
        return InputHelpers.GetMovementInput().Length() > 0 ? groundMovement : idleState;
    }

    protected override void OnUpdate(float delta)
    {
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
