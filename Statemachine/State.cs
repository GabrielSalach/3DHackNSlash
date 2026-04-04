using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class State : Node
{
    [Export] public string animationName;
    
    public State parent;
    protected State activeState;

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
    protected StateMachineContext Context => Machine.context;

    
    
    
    protected abstract State GetInitialState();
    protected abstract void SetupTransitions();
    protected delegate bool TransitionCondition();
    private Dictionary<State, Dictionary<State, TransitionCondition>> transitions = new Dictionary<State, Dictionary<State, TransitionCondition>>();
    
    
    //##########################################################
    //################# LIFECYCLE ##############################
    //##########################################################

    public override void _Ready()
    {
        parent ??= GetParentOrNull<State>();
        SetupTransitions();
    }

    protected virtual void OnEnter() {}
    protected virtual void OnUpdate(float delta) {}
    protected virtual void OnUpdatePhysics(float delta) {}
    protected virtual void OnExit() {}

    internal void Enter()
    {
        if (parent != null)
        {
            parent.activeState = this;
        }
        OnEnter();
        if (!string.IsNullOrEmpty(animationName))
        {
            Machine.context.animationPlayer.Play(animationName);
        }
        State init = GetInitialState();
        if (init != null)
        {
            init.Enter();
        }
    }

    internal void Update(float deltaTime)
    {
        if (activeState != null)
        {
            if (TryGetTargetState(out State state))
            {
                Machine.sequencer.RequestTransition(activeState, state);
            }
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
    
    //##########################################################
    //################# HELPERS ################################
    //##########################################################


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

    protected bool TryAddTransition(State from, State to, TransitionCondition condition)
    {
        if (transitions.TryGetValue(from, out Dictionary<State, TransitionCondition> t))
        {
            return t.TryAdd(to, condition);
        }
        transitions[from] = new Dictionary<State, TransitionCondition>
        {
            {to, condition}
        };
        return true;
    }

    protected void AddTransition(State from, State to, TransitionCondition condition)
    {
        if (!TryAddTransition(from, to, condition))
        {
            throw new Exception("Couldn't add transition " + from + " to " + to);
        }
    }

    private bool TryGetTargetState(out State state)
    {
        if(activeState != null && transitions.TryGetValue(activeState, out Dictionary<State, TransitionCondition> currentTransitions))
        {
            foreach (KeyValuePair<State, TransitionCondition> transition in currentTransitions)
            {
                if (transition.Value())
                {
                    state = transition.Key;
                    return true;
                }
            }
        }
        
        state = null;
        return false;
    }
}
