using System;
using Godot;

[GlobalClass]
public partial class StateDriver : CharacterBody3D
{
    private StateMachine stateMachine;
	
    [Export] protected State rootState;
    [Export] protected ActionMap actionMap;
    protected StateMachineContext context;

    public override void _Ready()
    {
        actionMap.BuildActionMap();
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
            actionMap = actionMap,
            characterBody = this
        };

        foreach (Node child in GetChildren())
        {
            switch (child)
            {
                case StateMachineAnimator at:
                {
                    if (context.animator != null) throw new Exception("Only one animation tree are allowed");
                    context.animator = at;
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
                case AttackController ac:
                {
                    if(context.attackController != null) throw new Exception("Only one attack controller are allowed");
                    context.attackController = ac;
                    break;
                }
            }
        }
        
        if(context.animator == null)
            throw new Exception("Missing an AnimationTree Node");
        if(context.combatEntity == null)
            throw new Exception("Missing a CombatEntity Node");
        if(context.modelRoot == null)
            throw new Exception("Missing a ModelRoot Node");
        if (context.attackController == null)
            throw new Exception("Missing an AttackController Node");

        context.spaceState = GetWorld3D().DirectSpaceState;
    }
}
