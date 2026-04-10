using System.Linq;
using Godot;

[GlobalClass]
public partial class PlayerStateDriver : StateDriver
{
	[Export] private SpringArm3D springArm;
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		context.MovementDirection = springArm.Transform.Basis * InputHelpers.GetMovementInputAsVector3();
		string path = string.Empty;
		foreach (State state in rootState.Leaf().PathToRoot().Reverse())
		{
			path += state.GetName();
			path = path[..^5];
			path += " > ";
		}
		path = path[..^3];
		DebugControl.instance.SetValue("leafNodePath", path);
	}
}
