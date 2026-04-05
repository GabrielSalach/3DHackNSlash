using Godot;

public enum AnimationPhase
{
    HIT,
    RECOVERY
}

[GlobalClass]
public partial class AttackState : State
{
    [Export] private State hit;
    [Export] private State recovery;

    protected override State GetInitialState() => hit;

    protected override void SetupTransitions()
    {
        AddTransition(hit, recovery, () => hit.IsCompleted());
    }
    
    public AnimationPhase CurrentPhase => activeState == hit ? AnimationPhase.HIT : AnimationPhase.RECOVERY;

    public override bool IsCompleted() => recovery.IsCompleted();
}
