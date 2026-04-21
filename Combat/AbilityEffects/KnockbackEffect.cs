
using Godot;

[GlobalClass]
public partial class KnockbackEffect : AbilityEffect
{
    [Export] private Vector3 knockbackStrength;
    
    public override void Execute(CombatEntity caster, CombatEntity target)
    {
        target.GetCharacterBody().Velocity = knockbackStrength;
    }
}
