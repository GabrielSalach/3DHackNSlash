
using Godot;

[GlobalClass]
public partial class DamageEffect : AbilityEffect
{
    [Export]
    public float amount;
    
    public override void Execute(CombatEntity caster, CombatEntity target)
    {
        target.components[typeof(Health)].CurrentValue -= amount;
    }
}
