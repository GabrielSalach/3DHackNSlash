
using Godot;

public static class NodeHelpers
{
    public static T GetChild<T>(Node node) where T : Node
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is T)
            {
                return (T)child;
            }
        }

        return null;
    }

    public static T GetOrCreateChild<T>(Node node) where T : Node, new()
    {
        T value = GetChild<T>(node);
        if (value == null)
        {
            value = new T();
            node.AddChild(value);
        }
        return value;
    }
}
