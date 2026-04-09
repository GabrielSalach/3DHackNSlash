
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
}
