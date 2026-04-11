using Godot;
using Godot.Collections;

public struct ActionStatus
{
    public bool IsJustPressed;
    public bool IsPressed;
    public bool IsJustReleased;
}

[GlobalClass]
public partial class ActionMap : Resource
{
    [Export] private Array<string> actions;
    private System.Collections.Generic.Dictionary<string, ActionStatus> actionMap;

    public void BuildActionMap()
    {
        actionMap = new System.Collections.Generic.Dictionary<string, ActionStatus>();
        foreach (string action in actions)
        {
            actionMap[action] = new ActionStatus();
        }
    }

    public ActionStatus this[string action]
    {
        get { return actionMap[action]; }
        set { actionMap[action] = value; }
    }


    // private Dictionary<string, string> actions = new Dictionary<string, string>();

    // public override Array<Dictionary> _GetPropertyList()
    // {
    //     var properties = new Array<Dictionary>();
    //     
    //     foreach (var action in actions)
    //     {
    //         properties.Add(new Dictionary
    //         {
    //             { "name", action.Key },
    //             { "type", (int)Variant.Type.String },
    //             { "usage", (int)PropertyUsageFlags.Default}
    //         });
    //     }
    //
    //     return properties;
    // }
    //
    // public override Variant _Get(StringName property)
    // {
    //     if (actions.TryGetValue(property, out string action))
    //         return action;
    //     return default;
    // }
    //
    // public override bool _Set(StringName property, Variant value)
    // {
    //     if (!actions.ContainsKey(property)) return false;
    //     actions[property] = value.AsString();
    //     return true;
    // }
    //
    // public void AddActionRequirement(string action)
    // {
    //     if (actions.ContainsKey(action)) return;
    //     actions.Add(action, "");
    //     NotifyPropertyListChanged();
    // }
}