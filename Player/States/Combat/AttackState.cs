using System.Collections.Generic;
using Godot;

public enum AnimationPhase
{
    ANTICIPATION,
    HIT,
    RECOVERY
}

[GlobalClass]
public partial class AttackState : State
{
    [Export] private Ability ability;
    [Export] private AnimationData animation;

    private Weapon weapon;
    private readonly List<CombatEntity> hitEntities = new List<CombatEntity>();
    private int currentFrame;

    
    public AnimationPhase CurrentPhase { get; private set; }
    public override bool IsCompleted() => currentFrame >= animation.hitEndFrame + 35;

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
        currentFrame = 0;
        CurrentPhase = AnimationPhase.ANTICIPATION;
        
        animationName = "Noizz/" + animation.animation.GetName();
        Context.animationPlayer.Play(animationName);
        
        weapon ??= ((CombatState)parent).WeaponReference;

        if (weapon != null)
        {
            weapon.OnEntityHit += OnHit;
        }
    }

    protected override void OnUpdate(float delta)
    {
        if (currentFrame == animation.anticipationEndFrame)
        {
            CurrentPhase = AnimationPhase.HIT;
            weapon?.ActivateHurtBox();
        }

        if (currentFrame == animation.hitEndFrame)
        {
            weapon?.DeactivateHurtBox();
        }

        if (currentFrame == animation.hitEndFrame + 15)
        {
            CurrentPhase = AnimationPhase.RECOVERY;
        }
        currentFrame++;
    }

    protected override void OnExit()
    {
        if (weapon != null)
        {
            weapon.OnEntityHit -= OnHit;
        }
    }
}
