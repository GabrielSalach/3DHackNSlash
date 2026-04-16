using System;
using Godot;

public enum AnimationPhase
{
    ANTICIPATION,
    HIT,
    RECOVERY
}

[GlobalClass]
public partial class AttackController : Node3D
{
    [Export] public AnimationPhase CurrentAnimationPhase;
    [Export] public Area3D hurtBox;

    public Action<CombatEntity> OnHit;

    public override void _Ready()
    {
        hurtBox.BodyEntered += _body =>
        {
            CombatEntity entity = NodeHelpers.GetChild<CombatEntity>(_body);
            if (entity != null)
            {
                OnHit?.Invoke(entity);
            }
        };
    }
}
