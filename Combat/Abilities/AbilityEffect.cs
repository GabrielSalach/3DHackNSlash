using Godot;

[GlobalClass]
public abstract partial class AbilityEffect : Resource
{
    public abstract void Execute(CombatEntity caster, CombatEntity target);
}