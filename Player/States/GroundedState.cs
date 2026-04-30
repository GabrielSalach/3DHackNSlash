using Godot;

[GlobalClass, Tool]
public partial class GroundedState : State
{
    [Export] public float groundFriction = 30;
    [Export] private State idleState;
    [Export] private State movementState;

    protected override State GetInitialState => idleState;

    public override void _Ready()
    {
        AddTransition(idleState, movementState, () => Context.MovementDirection.Length() > 0); 
        AddTransition(movementState, idleState, () => Context.MovementDirection.Length() <= 0); 
    }

    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 vel = Context.characterBody.Velocity;
        vel.X = Mathf.MoveToward(vel.X, 0, groundFriction * delta);
        vel.Z = Mathf.MoveToward(vel.Z, 0, groundFriction * delta);
        Context.characterBody.Velocity = vel;
    }
    
    
}