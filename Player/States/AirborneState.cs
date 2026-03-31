
using Godot;

[GlobalClass]
public partial class AirborneState : State
{
    protected override void OnUpdatePhysics(float delta)
    {
        Machine.context.characterBody.AddVelocity(Machine.context.characterBody.GetGravity() * delta);
    }
}
