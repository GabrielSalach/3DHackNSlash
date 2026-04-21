
using Godot;

[GlobalClass]
public partial class UpwardStrikeState : AttackState
{
    [Export] private float jumpHeight = 5.0f;

    protected override void OnUpdatePhysics(float delta)
    {
        ApplyRootMotion(1, jumpHeight);
    }

    protected override void OnExit()
    {
        base.OnExit();
        Context.characterBody.Velocity = Vector3.Zero;
    }
}
