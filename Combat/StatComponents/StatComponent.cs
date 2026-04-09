using System.ComponentModel;
using Godot;

[GlobalClass, Tool]
public abstract partial class StatComponent : Node
{
    public float MaxValue { get; set; }

    public float CurrentValue
    {
        get => currentValue;
        set
        {
            currentValue = value;
            NotifyPropertyListChanged();
        }
    }

    private float currentValue;
    
    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        var properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();

        properties.Add(new Godot.Collections.Dictionary
        {
            { "name", "MaxValue" },
            { "type", (int)Variant.Type.Float },
            { "usage", (int)PropertyUsageFlags.Default }
        });
        properties.Add(new Godot.Collections.Dictionary
        {
            { "name", "CurrentValue" },
            { "type", (int)Variant.Type.Float },
            { "usage", (int)PropertyUsageFlags.Default | (int)PropertyUsageFlags.ReadOnly }
        });

        return properties;
    }
    public override Variant _Get(StringName property)
    {
        if (property == "CurrentValue")
            return CurrentValue;
        if(property == "MaxValue")
            return MaxValue;

        return default;
    }
}