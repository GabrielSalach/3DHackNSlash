
using Godot;

public enum AttackAnimationPhase
{
    ATTACK, 
    RECOVERY,
    ENDED
}

[GlobalClass]
public partial class AttackState : State
{
    [Export] private State attackState;
    [Export] private State recoveryState;

    protected override State GetInitialState() => attackState;
    public AttackAnimationPhase GetAnimationPhase()
    {
        if (activeState == recoveryState || Context.animationPlayer.CurrentAnimationPosition >=
            Context.animationPlayer.CurrentAnimationLength)
        {
            return AttackAnimationPhase.ENDED;
        }
        return activeState == attackState ? AttackAnimationPhase.ATTACK : AttackAnimationPhase.RECOVERY;
    }


    protected override State GetTransition()
    {
        if (Context.animationPlayer.CurrentAnimation == attackState.animationName)
        {
            if (Context.animationPlayer.CurrentAnimationPosition >= Context.animationPlayer.CurrentAnimationLength)
            {
                return recoveryState;
            }
        }

        return attackState;
    }
}
