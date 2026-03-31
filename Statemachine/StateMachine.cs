
using System.Collections.Generic;
using Godot;

public class StateMachine
{
    private bool started;
    
    public State root;
    public readonly TransitionSequencer sequencer;
    public StateMachineContext context;

    public StateMachine(State root, StateMachineContext context)
    {
        this.root = root;
        sequencer = new TransitionSequencer(this);
        this.context = context;
    }

    public void Start()
    {
        if (started)
        {
            return;
        }
        
        started = true;
        root.Enter();
    }

    public void Tick(float deltaTime)
    {
        if(!started) {
            Start();
        }
        InternalTick(deltaTime);
    }

    public void PhysicsTick(float deltaTime)
    {
        if(!started) {
            Start();
        }
        InternalPhysicsTick(deltaTime);
    }
    
    internal void InternalTick(float deltaTime) => root.Update(deltaTime);
    internal void InternalPhysicsTick(float deltaTime) => root.UpdatePhysics(deltaTime);

    public void ChangeState(State from, State to)
    {
        // GD.Print($"changed  state from {from.GetType().Name} to {to.GetType().Name}");
        State lca = TransitionSequencer.LowestCommonAncestor(from, to);
        for (State state = from; state != lca; state = state.parent)
        {
            state.Exit();
        }
        
        var stack = new Stack<State>();
        for (State state = to; state != lca; state = state.parent)
        {
            stack.Push(state);
        }

        while (stack.Count > 0)
        {
            stack.Pop().Enter();
        }
    }
}