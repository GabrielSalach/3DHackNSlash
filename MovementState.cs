
using Godot;

[GlobalClass]
public partial class MovementState : State
{
    [Export]
    private CharacterBody3D character;
    
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;


    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 velocity = character.Velocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector3 direction = (character.Transform.Basis * new Vector3(machine.context.input.X, 0, machine.context.input.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(character.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(character.Velocity.Z, 0, Speed);
        }

        character.Velocity = velocity;
        character.MoveAndSlide();
    }
}
