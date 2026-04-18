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
        HashSet<State> aParents = new HashSet<State>();
        for (State state = a; state != null; state = state.parent)
        {
            aParents.Add(state);
        }
        
        for (State state = b; state != null; state = state.parent)
        {
            if (aParents.Contains(state))
            {
                return state;
            }
        }

        return null;
    }
}