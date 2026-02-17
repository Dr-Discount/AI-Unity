using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine
{
    Dictionary<string, AIState> states = new Dictionary<string, AIState>();

    public AIState currentState {  get; private set; }

    public void AddState(AIState state)
    {
        if (states.ContainsKey(state.Name)) { Debug.LogError("State Machine already contain state " + state.Name); return; }
        states[state.Name] = state;
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }

    public void SetState<T>()
    {
        SetState(typeof(T).Name);
    }

    public void SetState(string name)
    {
        if (!states.ContainsKey(name)) { Debug.LogError("State Machine does not contains state " + name); return; }

        var nextState = states[name];
        if (nextState == currentState)
            return;
        
        currentState?.OnExit();
        currentState = nextState;
        currentState?.OnEnter();
        currentState?.OnUpdate();
    }
}