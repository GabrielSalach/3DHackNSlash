using System.Collections.Generic;

public class TransitionSequencer
{
    public readonly StateMachine machine;

    public TransitionSequencer(StateMachine stateMachine)
    {
        machine = stateMachine;
    }
    
    public void RequestTransition(State from, State to)
    {
        machine.ChangeState(from, to);    
    }
    
    public static State LowestCommonAncestor(State a, State b)
    {
        HashSet<State> ap = new HashSet<State>();
        for (State state = a; state != null; state = state.parent)
        {
            ap.Add(state);
        }
        
        for (State state = b; state != null; state = state.parent)
        {
            if (ap.Contains(state))
            {
                return state;
            }
        }

        return null;
    }
}