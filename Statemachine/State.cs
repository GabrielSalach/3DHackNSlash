using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

[GlobalClass]
public abstract partial class State : Node
{
    public State parent;
    public State activeState;

    private StateMachine stateMachine;
    
    public StateMachine Machine
    {
        get => stateMachine;
        set
        {
            foreach (Node child in GetChildren())
            {
                if (child is State state)
                {
                    state.Machine = value;
                }
            }
            stateMachine = value;
        }
    }

    protected virtual void OnEnter() {}
    protected virtual void OnUpdate(float delta) {}
    protected virtual void OnUpdatePhysics(float delta) {}
    protected virtual void OnExit() {}

    protected virtual State GetInitialState() => null;
    protected virtual State GetTransition() => null;
    
    internal void Enter()
    {
        if (parent != null)
        {
            parent.activeState = this;
        }
        OnEnter();
        State init = GetInitialState();
        if (init != null)
        {
            init.Enter();
        }
    }

    internal void Update(float deltaTime)
    {
        State t = GetTransition();
        if (t != null && t != activeState)
        {
            Machine.sequencer.RequestTransition(activeState, t);
            return;
        }

        if (activeState != null)
        {
            activeState.Update(deltaTime);
        }
        OnUpdate(deltaTime);
    }

    internal void UpdatePhysics(float deltaTime)
    {
        if (activeState != null)
        {
            activeState.UpdatePhysics(deltaTime);
        }
        OnUpdatePhysics(deltaTime); 
    }

    internal void Exit()
    {
        if (activeState != null)
        {
            activeState.Exit();
        }

        activeState = null;
        OnExit();
    }

    public State Leaf()
    {
        State s = this;
        while (s.activeState != null)
        {
            s = s.activeState;
        }

        return s;
    }

    public IEnumerable<State> PathToRoot()
    {
        for (State s = this; s != null; s = s.parent)
        {
            yield return s;
        }
    }
}
