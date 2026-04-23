using Godot;

[GlobalClass]
public abstract partial class VirtualCamera : Node3D
{
    [Export] public bool Solo { get; protected set; }
    [Export] public int Priority { get; protected set; }
    [Export] public float FOV { get; protected set; }

    public override void _Ready()
    {
        CameraController3D.Instance.RegisterVirtualCamera(this);
    }
}
