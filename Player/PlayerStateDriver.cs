using System.Linq;
using Godot;

[GlobalClass]
public partial class PlayerStateDriver : StateDriver
{
	[Export] private SpringArm3D springArm;
	
	public override void _Process(double delta)
	{
		context.MovementDirection = springArm.Transform.Basis * InputHelpers.GetMovementInputAsVector3();
		ProcessInput("jump");
		ProcessInput("dash");
		ProcessInput("light_attack");
		ProcessInput("heavy_attack");
		ProcessInput("aim");
		ProcessInput("shoot");
		
		base._Process(delta);
		
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

	private void ProcessInput(string action)
	{
		actionMap[action] = new ActionStatus
		{
			IsJustPressed = Input.IsActionJustPressed(action),
			IsPressed = Input.IsActionPressed(action),
			IsJustReleased = Input.IsActionJustReleased(action),
		};
	}
	
}
