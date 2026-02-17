using UnityEngine;

public class AIPatrolState : AIState
{
    public AIPatrolState(StateAgent aganet) : base(aganet) { }

    public override void OnEnter()
    {
        agent.Destination = NavNode.GetRandomNavNode().transform.position;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        if (agent.distanceToDestonation <= 0.5f)
        {
            agent.stateMachine.SetState<AIIdleState>();
        }

        if (agent.enemy != null) //agent.stateMachine>setState<AIChaseState>();
    }
}