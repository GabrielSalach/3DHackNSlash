using System.Collections.Generic;
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

    private Weapon weapon;
    private readonly List<CombatEntity> hitEntities = new List<CombatEntity>();
    
    protected override State GetInitialState() => hit;

    protected override void SetupTransitions()
    {
        AddTransition(hit, recovery, () => hit.IsCompleted());
    }
    
    public AnimationPhase CurrentPhase => activeState == hit ? AnimationPhase.HIT : AnimationPhase.RECOVERY;

    public override bool IsCompleted() => recovery.IsCompleted();

    
    private void OnHit(CombatEntity hitEntity)
    {
        if (ability != null && hitEntity != null)
        {
            if (!hitEntities.Contains(hitEntity))
            {
                GD.Print($"Hit {hitEntity.Name}");
                ability.Execute(Context.combatEntity, hitEntity);
                hitEntities.Add(hitEntity);
            }
        }
    }

    protected override void OnEnter()
    {
        hitEntities.Clear();
        if (weapon == null)
        {
            weapon = ((CombatState)parent).WeaponReference;
        }

        if (weapon != null)
        {
            weapon.OnEntityHit += OnHit;
            SceneTreeTimer anticipationTimer = GetTree().CreateTimer(0.2);
            anticipationTimer.Timeout += () =>
            {
                weapon.ActivateHurtBox();
            };
        }
    }

    protected override void OnUpdate(float delta)
    {
        if (CurrentPhase == AnimationPhase.RECOVERY)
        {
            weapon.DeactivateHurtBox();
        }
    }

    protected override void OnExit()
    {
        if (weapon != null)
        {
            weapon.OnEntityHit -= OnHit;
        }
    }
}
