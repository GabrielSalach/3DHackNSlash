using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class AttackState : AnimationState
{
    [Export] private Ability ability;

    private readonly List<CombatEntity> hitEntities = new List<CombatEntity>();

    
    private void OnHit(CombatEntity hitEntity)
    {
        if (ability != null && hitEntity != null)
        {
            if (!hitEntities.Contains(hitEntity))
            {
                ability.Execute(Context.combatEntity, hitEntity);
                hitEntities.Add(hitEntity);
            }
        }
    }

    protected override void OnEnter()
    {
        hitEntities.Clear();
        Context.attackController.OnHit += OnHit;
    }

    protected override void OnExit()
    {
        Context.attackController.OnHit -= OnHit;
    }
}
