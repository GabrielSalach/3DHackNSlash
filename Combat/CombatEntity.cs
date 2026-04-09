using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CombatEntity : Node
{
    public Dictionary<Type, StatComponent> components = new Dictionary<Type, StatComponent>();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is StatComponent statComponent)
            {
                statComponent.CurrentValue = statComponent.MaxValue;
                components.Add(statComponent.GetType(), statComponent);
            }
        }
    }
    
    public float GetStatCurrentValue<TStat>() where TStat : StatComponent
    {
        return components[typeof(TStat)].CurrentValue;
    }
}