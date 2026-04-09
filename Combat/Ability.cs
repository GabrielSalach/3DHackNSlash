using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Ability : Resource
{
    [Export]
    public Array<AbilityEffect> Effects;

    public void Execute(CombatEntity caster, CombatEntity target)
    {
        foreach (AbilityEffect effect in Effects)
        {
            effect.Execute(caster, target);
        }
    }
}