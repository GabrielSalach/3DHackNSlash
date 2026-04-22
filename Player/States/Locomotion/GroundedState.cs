using Godot;

[GlobalClass]
public partial class GroundedState : State
{
    [Export] private float jumpForce;
    [Export] private float friction = 5.0f;
    
    [ExportCategory("Child States")]
    [Export] private State idleState;
    [Export] private MovementState groundMovement;
    [Export] private AnimationState landingState;

    public bool fromAirborne = false;
    
    protected override State GetInitialState() => idleState;
    protected override void SetupTransitions()
    {
        AddTransition(idleState, landingState, () => fromAirborne);
        AddTransition(idleState, groundMovement, () => Context.MovementDirection.Length() > 0);
        AddTransition(groundMovement, idleState, () => Context.MovementDirection.Length() <= 0);
        AddTransition(landingState, idleState, () => landingState.IsCompleted);
    }

    protected override void OnChildrenTransition(State from, State to)
    {
        if (to == landingState)
        {
            fromAirborne = false;
        }
    }

    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 vel = Context.characterBody.Velocity;
        if (Context.actionMap["jump"].IsJustPressed)
        {
            vel.Y = jumpForce;
        }
        vel.X = Mathf.MoveToward(vel.X, 0, friction * delta);
        vel.Z = Mathf.MoveToward(vel.Z, 0, friction * delta);
        Context.characterBody.Velocity = vel;
    }
}
