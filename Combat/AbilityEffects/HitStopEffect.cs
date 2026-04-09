
using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class HitStopEffect : AbilityEffect
{
    [Export] private double timeScale;
    [Export] private float duration;
    
    public override async void Execute(CombatEntity caster, CombatEntity target)
    {
        double regularTimeScale = Engine.TimeScale;
        Engine.TimeScale = timeScale;
        await ToSignal(caster.GetTree().CreateTimer(duration, true, false, true), "timeout");
        Engine.TimeScale = regularTimeScale;
        
    }
}
