
using Godot;

[GlobalClass]
public partial class MovementState : State
{
    [Export]
    public float Speed = 5.0f;
    [Export]
    public float JumpVelocity = 4.5f;

    [Export] public float rotationSpeed = 5.0f;


    protected override void OnUpdatePhysics(float delta)
    {
        Vector3 direction = (Context.springArm.Transform.Basis * InputHelpers.GetMovementInputAsVector3()).Normalized();
        Context.characterBody.AddVelocity(direction * Speed);
        
        OrientModelToVelocity(rotationSpeed * delta);
    }

    private void OrientModelToVelocity(float weight)
    {
        Vector3 direction = Context.characterBody.Velocity.Normalized();
        Vector3 leftAxis = Vector3.Up.Cross(direction);
        Quaternion rotationBasis = new Basis(leftAxis, Vector3.Up, direction).GetRotationQuaternion();
        Context.modelRoot.Basis = new Basis(Context.modelRoot.Transform.Basis.GetRotationQuaternion().Slerp(rotationBasis, weight));
        
        // float targetAngle = Context.modelRoot.Basis.Z.AngleTo(Context.characterBody.Velocity.Normalized());
        // targetAngle -= Mathf.Pi;
        // DebugControl.instance.SetValue("angle", $"{Mathf.RadToDeg(targetAngle)}");
        // float angle = Mathf.LerpAngle(Context.modelRoot.Rotation.Y, targetAngle, weight);
        // Context.modelRoot.SetRotation(new Vector3(Context.modelRoot.Rotation.X, angle, Context.modelRoot.Rotation.Z));
    }
}
