using Godot;
using Godot.Collections;
using Vector3 = Godot.Vector3;

[GlobalClass, Tool]
public partial class OrbitalCamera : VirtualCamera
{
    [Export] public Node3D Target { get; set; }
    [Export] private float ArmLength { get; set; }
    [Export] private float Sensitivity { get; set; } = 0.003f;
    [Export] private float PitchMin { get; set; } = -89f;
    [Export] private float PitchMax { get; set; } = 89f;
    [Export] public Vector3 Offset { get; set; }
    [Export] public Vector3 Damping { get; set; } = Vector3.One * 0.1f;
    [Export(PropertyHint.Layers3DPhysics)] private uint CollisionMask { get; set; }

    private Vector3 dampedTarget;

    private OrbitalCamera()
    {
        TopLevel = true;
    }

    private float pitch;
    private float yaw;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            yaw   -= motion.Relative.X * Sensitivity;
            pitch += motion.Relative.Y * Sensitivity;

            pitch = Mathf.Clamp(pitch, Mathf.DegToRad(PitchMin), Mathf.DegToRad(PitchMax));
        }
    }

    public override void _Ready()
    {
        base._Ready();
        
        if (Target == null) return;
        
        dampedTarget = Target.GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null) return;
        
        Vector3 dampingFactor;
        dampingFactor.X = Damping.X > 0 ? 1.0f - Mathf.Exp(-1/Damping.X * (float)delta) : 1;
        dampingFactor.Y = Damping.Y > 0 ? 1.0f - Mathf.Exp(-1/Damping.Y * (float)delta) : 1;
        dampingFactor.Z = Damping.Z > 0 ? 1.0f - Mathf.Exp(-1/Damping.Z * (float)delta) : 1;

        Vector3 targetPosition = Target.GlobalPosition + Target.Transform.Basis * Offset;
        dampedTarget.X = Mathf.Lerp(dampedTarget.X, targetPosition.X, dampingFactor.X);
        dampedTarget.Y = Mathf.Lerp(dampedTarget.Y, targetPosition.Y, dampingFactor.Y);
        dampedTarget.Z = Mathf.Lerp(dampedTarget.Z, targetPosition.Z, dampingFactor.Z);

        Vector3 cameraPosition = dampedTarget;
        
        cameraPosition += new Vector3(
            ArmLength * Mathf.Cos(pitch) * Mathf.Sin(yaw - Mathf.Pi),
            ArmLength * Mathf.Sin(pitch),
            ArmLength * Mathf.Cos(pitch) * Mathf.Cos(yaw - Mathf.Pi)
        );
        
        // SpringArm logic
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(dampedTarget, cameraPosition);
        query.CollisionMask = CollisionMask;
        Dictionary result = GetWorld3D().GetDirectSpaceState().IntersectRay(query);
        if (result.Count > 0)
        {
            cameraPosition = result["position"].AsVector3();
        }
        
        GlobalPosition = cameraPosition;
        LookAt(dampedTarget, Vector3.Up);
    }
}
