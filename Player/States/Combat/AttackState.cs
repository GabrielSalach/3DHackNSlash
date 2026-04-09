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
    [Export] private Ability ability;
    [Export] private CombatEntity target;

    protected override State GetInitialState() => hit;

    protected override void OnEnter()
    {
        if (ability != null && target != null)
        {
            ability.Execute(Context.combatEntity, target);
        }
    }

    protected override void SetupTransitions()
    {
        AddTransition(hit, recovery, () => hit.IsCompleted());
    }
    
    public AnimationPhase CurrentPhase => activeState == hit ? AnimationPhase.HIT : AnimationPhase.RECOVERY;

    public override bool IsCompleted() => recovery.IsCompleted();
}
