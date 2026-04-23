using Godot;

[GlobalClass, Tool]
public partial class OrbitalCamera : VirtualCamera
{
    [Export] public Node3D Target { get; set; }
    [Export] public Vector3 Offset { get; set; }
    [Export] public Vector3 Damping { get; set; }

    private float pitch;
    private float yaw;

    public override void _EnterTree()
    {
        TopLevel = true;
    }

    public void Rotate(float pitch, float yaw)
    {
        
    }
}
