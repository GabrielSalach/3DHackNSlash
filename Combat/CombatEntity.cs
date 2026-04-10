using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class CombatEntity : Node
{
    private readonly Dictionary<Type, StatComponent> components = new Dictionary<Type, StatComponent>();

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is StatComponent statComponent)
            {
                components.Add(statComponent.GetType(), statComponent);
            }
        }
    }
    
    public StatComponent GetStat<TStat>() where TStat : StatComponent
    {
        if (components.TryGetValue(typeof(TStat), out StatComponent statComponent))
        {
            return statComponent;
        }

        throw new ArgumentOutOfRangeException($"{Name} doesn't have a {typeof(TStat).Name} component");
    }
}