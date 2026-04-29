
using Godot;

public class StateMachineContext()
{
    private Vector3 movementDirection = Vector3.Zero;
    public Vector3 MovementDirection
    {
        get => movementDirection;
        set => movementDirection = value.Length() > 1 ? value.Normalized() : value;
    }
    public ActionMap actionMap;
    public CharacterBody3D characterBody;
    public CombatEntity combatEntity;
    public StateMachineAnimator animator;
}
