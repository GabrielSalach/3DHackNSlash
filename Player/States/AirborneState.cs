
using Godot;

[GlobalClass]
public partial class AirborneState : State
{
    
    
    protected override void OnUpdatePhysics(float delta)
    {
        Machine.context.characterBody.Velocity += Machine.context.characterBody.GetGravity() * delta;
    }
}
