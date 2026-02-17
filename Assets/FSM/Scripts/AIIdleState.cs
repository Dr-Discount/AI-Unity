using UnityEngine;

public class AIIdleState : AIState
{
    public AIIdleState(StateAgent aganet) : base(aganet) { }

    public override void OnEnter()
    {
        agent.timer = 2.0f;
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        if (agent.timer < 0)
        {
            agent.stateMachine.SetState<AIPatrolState>();
        }
    }
}
