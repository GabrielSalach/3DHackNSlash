using Godot;

[GlobalClass]
public partial class PrintNameEffect : AbilityEffect
{
    [Export] public string AttackName;

    public override void Execute(CombatEntity caster, CombatEntity target)
    {
        GD.Print($"{caster.Name} has struck {target.Name} using {AttackName}");
    }
}