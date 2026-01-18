using System;
using Unity.VisualScripting;
using UnityEngine;

public class AutonomousAgent : AI_Agent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;

    [Header("Wander")]
    [SerializeField] float wanderRadius = 1;
    [SerializeField] float wanderDistance = 1;
    [SerializeField] float wanderDisplacement = 1;
    float wanderAngle = 0.0f;

    void Start()
    {
        wanderAngle = UnityEngine.Random.Range(0, 360);
    }

    void Update()
    {
        bool hasTarget = false;

        if (seekPerception != null)
        {
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                hasTarget = true;
                Vector3 force = Seek(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        if (fleePerception != null)
        {
            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Flee(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        if (!hasTarget)
        {
            Vector3 force = Wander();
            movement.ApplyForce(force);
        }
        //foreach (var go in gameObjects)
        //{
        // Debug.DrawLine(transform.position, go.transform.position);
        //}

        transform.position = Utilities.Wrap(transform.position, new Vector3(-15, -15, -15), new Vector3(15, 15, 15));
        if (movement.Velocity.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(movement.Velocity, Vector3.up);
        }
    }

    Vector3 Seek(GameObject go)
    {
        Vector3 direction = go.transform.position - transform.position;
        Vector3 force = GetSteeringForce(direction);

        return force;
    }

    Vector3 Flee(GameObject go)
    {
        Vector3 direction = transform.position - go.transform.position;
        Vector3 force = GetSteeringForce(direction);

        return force;
    }

    Vector3 Wander()
    {
        wanderAngle += UnityEngine.Random.Range(-wanderDisplacement, wanderDisplacement);
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        
        Vector3 pointOnCircle = rotation * (Vector3.forward * wanderRadius);
        Vector3 circleCenter = movement.Velocity.normalized * wanderDistance;
        
        Vector3 force = GetSteeringForce(circleCenter + pointOnCircle);

        Debug.DrawLine(transform.position, transform.position + circleCenter, Color.blue);
        Debug.DrawLine(transform.position, transform.position + circleCenter + pointOnCircle, Color.red);
        
        return force;
    }

    Vector3 GetSteeringForce(Vector3 direction)
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steer = desired - movement.Velocity;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        return force;
    }
}
