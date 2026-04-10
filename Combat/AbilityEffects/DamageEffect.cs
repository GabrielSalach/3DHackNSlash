using Godot;

[GlobalClass]
public partial class DamageEffect : AbilityEffect
{
    [Export]
    public int amount;
    
    public override void Execute(CombatEntity caster, CombatEntity target)
    {
        // target.components[typeof(Health)].CurrentValue -= amount;
        target.GetStat<Health>().CurrentValue -= amount;
    }
}
