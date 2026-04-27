using Godot;

[GlobalClass, Tool]
public abstract partial class VirtualCamera : Node3D
{
	[Export] public int Priority { get; set; }
	[Export] public float FOV { get; set; } = 75.0f;
	
	public CameraController3D Controller { get; set; }

	public override void _EnterTree()
	{
		AddToGroup("VirtualCameras");
	}
}
