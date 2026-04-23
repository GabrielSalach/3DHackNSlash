using Godot;

[GlobalClass, Tool]
public partial class OrbitalCamera : VirtualCamera
{
    [Export] public Node3D Target { get; set; }
    [Export] public Vector3 Offset { get; set; }
    [Export] public Vector3 Damping { get; set; }
    [Export] private float ArmLength { get; set; }
    [Export(PropertyHint.Layers3DPhysics)] private uint CollisionMask { get; set; }

    private OrbitalCamera()
    {
        TopLevel = true;
    }

    private float pitch;
    private float yaw;

    public void Rotate(float pitch, float yaw)
    {
        
    }

    public override void _Process(double delta)
    {
        if (Target == null)
        {
            return;
        }
        
        Vector3 targetPosition = Target.Position;

        targetPosition.Z -= ArmLength;
        LookAt(Target.GlobalPosition);
        
        GlobalPosition = targetPosition;
    }
}
