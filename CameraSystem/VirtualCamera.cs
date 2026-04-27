using Godot;

[GlobalClass, Tool]
public abstract partial class VirtualCamera : Node3D
{
	[Export] public int Priority { get; set; }
	[Export] public float FOV { get; set; } = 75.0f;

	
	public override void _Ready()
	{
		AddToGroup("VirtualCameras");
	}
}
