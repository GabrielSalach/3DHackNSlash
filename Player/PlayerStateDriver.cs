using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class PlayerStateDriver : Node
{
	
	private StateMachine stateMachine;
	
	[Export] private State rootState;
	private StateMachineContext context;
	[ExportGroup("StateMachineContext")]
	[Export] PhysicsCharacterBody characterBody;
	
	public override void _Ready()
	{
		context = new StateMachineContext(characterBody);
		stateMachine = new StateMachine(rootState, context);
		rootState.Machine = stateMachine;
	}

	public override void _Process(double delta)
	{
		context.input = Vector2.Zero;
		if (Input.IsActionPressed("forward"))
		{
			context.input.Y -= 1;
		}

		if (Input.IsActionPressed("back"))
		{
			context.input.Y += 1;
		}

		if (Input.IsActionPressed("left"))
		{
			context.input.X -= 1;
		}

		if (Input.IsActionPressed("right"))
		{
			context.input.X += 1;
		}
		
		stateMachine.Tick((float)delta);
		string path = string.Empty;
		foreach (State state in rootState.Leaf().PathToRoot().Reverse())
		{
			path += state.GetType().Name;
			path = path[..^5];
			path += " > ";
		}
		path = path[..^3];
		DebugControl.instance.SetValue("leafNodePath", path);
	}

	public override void _PhysicsProcess(double delta)
	{
		stateMachine.PhysicsTick((float)delta);
	}
}
