using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public abstract partial class StatComponent : Node
{
    public int MaxValue { get; set; }

    public int CurrentValue
    {
        get => currentValue;
        set
        {
            int delta = value - currentValue;
            currentValue = value;
            NotifyPropertyListChanged();
            OnValueChanged?.Invoke(delta);
        }
    }

    /// <summary>
    /// Passes the delta of the value
    /// </summary>
    public Action<int> OnValueChanged;

    private int currentValue;

    
    public override Array<Dictionary> _GetPropertyList()
    {
        var properties = new Array<Dictionary>();

        properties.Add(new Dictionary
        {
            { "name", "MaxValue" },
            { "type", (int)Variant.Type.Int },
            { "usage", (int)PropertyUsageFlags.Default }
        });
        properties.Add(new Dictionary
        {
            { "name", "CurrentValue" },
            { "type", (int)Variant.Type.Int },
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

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        currentValue = MaxValue;
        NotifyPropertyListChanged();
    }
}