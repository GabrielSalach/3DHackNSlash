
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

[GlobalClass]
public partial class SwordState : State
{
    public enum AttackType
    {
        NONE = 0,
        LIGHT,
        HEAVY
    }
    
    [Export] private AttackState lightAttackA;
    [Export] private AttackState lightAttackB;
    [Export] private AttackState heavyAttack;

    private AttackState activeAttack => activeAttack as AttackState;

    protected override State GetTransition()
    {
        if (IsAcceptingInput())
        {
            
        }

        return GetParentOrNull<State>();
    }

    private bool IsAcceptingInput()
    {
        return activeAttack.GetAnimationPhase() == AttackAnimationPhase.RECOVERY ||
               activeAttack.GetAnimationPhase() == AttackAnimationPhase.ENDED;
    }
}
