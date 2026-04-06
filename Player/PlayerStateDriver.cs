using System.Linq;
using Godot;

[GlobalClass]
public partial class PlayerStateDriver : Node
{
	
	private StateMachine stateMachine;
	
	[Export] private State rootState;
	private StateMachineContext context;
	[ExportGroup("StateMachineContext")]
	[Export] private CharacterBody3D characterBody;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private SpringArm3D springArm;
	[Export] private PlayerModel modelRoot;
	
	public override void _Ready()
	{
		context = new StateMachineContext
		{
			animationPlayer = animationPlayer,
			characterBody = characterBody,
			springArm = springArm,
			modelRoot = modelRoot
		};
		stateMachine = new StateMachine(rootState, context);
		rootState.Machine = stateMachine;
	}

	public override void _Process(double delta)
	{
		stateMachine.Tick((float)delta);
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

	public override void _PhysicsProcess(double delta)
	{
		stateMachine.PhysicsTick((float)delta);
	}
}
