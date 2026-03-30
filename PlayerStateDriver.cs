using Godot;
using System;

[GlobalClass]
public partial class PlayerStateDriver : Node
{
    
    private StateMachine stateMachine;
    
    [Export] private State rootState;
    [Export] private StateMachineContext context;
    
    public override void _Ready()
    {
        stateMachine = new StateMachine(rootState, context);
        
        foreach (Node node in GetChildren())
        {
            if (node is State state)
            {
                state.machine = stateMachine;
            }
        }
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
    }

    public override void _PhysicsProcess(double delta)
    {
        stateMachine.PhysicsTick((float)delta);
    }
}


