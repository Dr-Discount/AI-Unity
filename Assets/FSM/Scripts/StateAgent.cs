using System.Timers;
using UnityEngine;

public class StateAgent : AI_Agent
{
    public Animator animator;

    public Movement movement;
    public Perception perception;

    [Header("Parameters")]
    public float timer;
    public float health;
    public float maxHealth = 100;
    public float distanceToDestonation;
    public AI_Agent enemy;

    public AIStateMachine stateMachine {  get; private set; } = new AIStateMachine();

    public Vector3 Destination
    {
        get { return movement.Destination; }
        set { movement.Destination = value; }
    }

    private void Start()
    {
        health = maxHealth;

        stateMachine.AddState(new AIIdleState(this));
        stateMachine.AddState(new AIPatrolState(this));

        stateMachine.SetState<AIIdleState>();
    }

    private void Update()
    {
        UpdateParameters();
        stateMachine.Update();
    }

    private void UpdateParameters()
    {
        timer -= Time.deltaTime;
        distanceToDestonation = Vector3.Distance(transform.position, Destination);
        var gameObjects = perception.GetGameObjects();
        if (gameObjects.Length > 0)
        {
            gameObjects[0].TryGetComponent<AI_Agent>(out enemy);
        }
        else
        {
            enemy = null;
        }
    }

    public void OnDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            stateMachine.SetState<AIDeadState>();
        } else
        {
            stateMachine.SetState<AIDamageState>();
        }
    }
}
