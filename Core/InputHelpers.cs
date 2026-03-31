
using Godot;

public static class InputHelpers
{
    public static Vector2 GetMovementInput()
    {
        return Input.GetVector("left", "right", "forward", "back");
    }

    public static Vector3 GetMovementInputAsVector3()
    {
        Vector2 input = GetMovementInput();
        return new Vector3(input.X, 0, input.Y); 
    }
}
