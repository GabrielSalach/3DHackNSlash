using Godot;

[GlobalClass, Tool]
public partial class FreeFallState : State
{
    protected override void OnUpdatePhysics(float delta)
    {
        Machine.context.characterBody.Velocity += Machine.context.characterBody.GetGravity() * delta;
    }
}