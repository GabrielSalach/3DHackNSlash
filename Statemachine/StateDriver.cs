using System;
using System.Reflection.PortableExecutable;
using Godot;

[GlobalClass]
public partial class StateDriver : CharacterBody3D
{
    private StateMachine stateMachine;
	
    [Export] protected State rootState;
    [ExportCategory("Context")]
    [Export] protected CombatEntity combatEntity;
    [Export] protected StateMachineAnimator animator;
    [Export] protected ActionMap actionMap;
    protected StateMachineContext context;

    public override void _Ready()
    {
        actionMap.BuildActionMap();
        try
        {
            InitializeContext();
        }
        catch (Exception e)
        {
            throw new Exception($"{GetName()}: Error initializing state machine", e);
        }

        stateMachine = new StateMachine(rootState, context);
        rootState.Machine = stateMachine;
    }

    public override void _Process(double delta)
    {
        stateMachine.Tick((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.PhysicsTick((float)delta);
    }

    private void InitializeContext()
    {
        context = new StateMachineContext
        {
            actionMap = actionMap,
            characterBody = this
        };

        
    }
}
