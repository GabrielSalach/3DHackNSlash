using Godot;

[GlobalClass, Tool]
public abstract partial class VirtualCamera : Node3D
{
	[Export] public int Priority { get; protected set; }
	[Export] public float FOV { get; protected set; }

	
	public override void _Ready()
	{
		AddToGroup("VirtualCameras");
	}
}
