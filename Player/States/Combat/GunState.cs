using Godot;

[GlobalClass]
public partial class GunState : State
{
    [ExportCategory("Child States")]
    [Export] private MovementState aimedMovementState;
    [Export] private State aimedIdleState;
    [Export] private State shootState;

    protected override State GetInitialState() => aimedIdleState;

    protected override void SetupTransitions()
    {
        AddTransition(aimedIdleState, shootState, () => Context.actionMap["shoot"].IsJustPressed);
        AddTransition(aimedIdleState, aimedMovementState,() => Context.MovementDirection.Length() > 0);
        
        AddTransition(aimedMovementState, shootState, () => Context.actionMap["shoot"].IsJustPressed);
        AddTransition(aimedMovementState, aimedIdleState, () => Context.MovementDirection.Length() <= 0);
        
        AddTransition(shootState, aimedIdleState, () => shootState.IsCompleted());
    }

    protected override void OnEnter()
    {
        Context.characterBody.Velocity = Vector3.Zero;
    }
}
