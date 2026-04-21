using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class State : Node
{
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

    public AnimationNodeBlendTree BlendTree { get; private set; }
    
    
    
    protected virtual State GetInitialState() => null;
    protected virtual void SetupTransitions() { }
    protected virtual AnimationNodeBlendTree SetupAnimationTree() => null;
    protected delegate bool TransitionCondition();
    private readonly Dictionary<State, Dictionary<State, TransitionCondition>> transitions = new Dictionary<State, Dictionary<State, TransitionCondition>>();

    public virtual bool IsCompleted => false;

    public virtual bool IsCancellable => true;
    
    //##########################################################
    //################# LIFECYCLE ##############################
    //##########################################################

    public override void _Ready()
    {
        parent ??= GetParentOrNull<State>();
        SetupTransitions();
        BlendTree = SetupAnimationTree();
        Initialize();
    }

    /// <summary>
    /// Method to override instead of _Ready() to initialize a state at runtime start.
    /// </summary>
    protected virtual void Initialize() { }
    protected virtual void OnEnter() {}
    protected virtual void OnUpdate(float delta) {}
    protected virtual void OnUpdatePhysics(float delta) {}
    protected virtual void OnExit() {}
    protected virtual void OnChildrenTransition(State from, State to) {}

    internal void Enter()
    {
        if (parent != null)
        {
            parent.activeState = this;
        }
        OnEnter();
        
        if(BlendTree != null)
            Context.animator.ConnectTree(BlendTree);
        
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
                OnChildrenTransition(activeState, state);
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

    protected void ApplyRootMotion(double delta, float scale = 1.0f)
    {
        Context.characterBody.Velocity = Context.animator.GetRootMotionPosition() * scale * (float)delta;
    }
}
