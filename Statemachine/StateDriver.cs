using System;
using Godot;

[GlobalClass]
public partial class StateDriver : CharacterBody3D
{
    private StateMachine stateMachine;
	
    [Export] protected State rootState;
    protected StateMachineContext context;

    public override void _Ready()
    {
        try
        {
            InitializeContextFromChildren();
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

    private void InitializeContextFromChildren()
    {
        context = new StateMachineContext
        {
            characterBody = this
        };

        foreach (Node child in GetChildren())
        {
            switch (child)
            {
                case AnimationPlayer ap:
                {
                    if (context.animationPlayer != null) throw new Exception("Only one animation player are allowed");
                    context.animationPlayer = ap;
                    break;

                }
                case CombatEntity ce:
                {
                    if (context.combatEntity != null) throw new Exception("Only one combat entity are allowed");
                    context.combatEntity = ce;
                    break;
                }
                case ModelRoot mr:
                {
                    if (context.modelRoot != null) throw new Exception("Only one model root are allowed");
                    context.modelRoot = mr;
                    break;
                }
            }
        }
        
        if(context.animationPlayer == null)
            throw new Exception("Missing an AnimationPlayer Node");
        if(context.combatEntity == null)
            throw new Exception("Missing a CombatEntity Node");
        if(context.modelRoot == null)
            throw new Exception("Missing a ModelRoot Node");
    }
}
