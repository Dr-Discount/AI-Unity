using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class AutonomousAgent : AI_Agent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception seekPerception;
    [SerializeField] Perception fleePerception;

    [Header("Wander")]
    [SerializeField] float wanderRadius = 1;
    [SerializeField] float wanderDistance = 1;
    [SerializeField] float wanderDisplacement = 1;

    [Header("Flock")]
    [SerializeField] Perception flockPerception;
    [SerializeField, Range(0, 5)] float cohesionWeight = 1;
    [SerializeField, Range(0, 5)] float separationWeight = 1;
    [SerializeField, Range(0, 5)] float alignmentWeight = 1;
    [SerializeField, Range(0, 5)] float separationRadius = 1;

    [Header("Obstacle")]
    [SerializeField] Perception obstaclePerception;
    [SerializeField, Range(0, 5)] float obstacleWeight = 1;

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

        if (flockPerception != null)
        {
	        var gameObjects = flockPerception.GetGameObjects();
	        if (gameObjects.Length > 0)
	        {
		        hasTarget = true;
		        movement.ApplyForce(Cohesion(gameObjects) * cohesionWeight);
		        movement.ApplyForce(Separation(gameObjects, separationRadius) * separationWeight);
		        movement.ApplyForce(Alignment(gameObjects) * alignmentWeight);
	        }
        }

        if (obstaclePerception != null &&
            obstaclePerception.GetGameObjectInDirection(transform.forward) != null)
        {
            Vector3 openDirection = Vector3.zero;
            if (obstaclePerception.GetOpenDirection(ref openDirection))
            {
                hasTarget = true;
                movement.ApplyForce(GetSteeringForce(openDirection) * obstacleWeight);
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
        // Project steering to XZ plane so agents don't get vertical forces
        direction.y = 0f;

        Vector3 desired = direction.normalized * movement.maxSpeed;

        // use a velocity with no vertical component for steering calculation
        Vector3 currentVel = movement.Velocity;
        currentVel.y = 0f;

        Vector3 steer = desired - currentVel;
        Vector3 force = Vector3.ClampMagnitude(steer, movement.maxForce);

        // ensure no vertical force leaks through
        force.y = 0f;

        return force;
    }

    private Vector3 Cohesion(GameObject[] neighbors)
    {
        Vector3 positions = Vector3.zero;
        int count = 0;
        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor == null || neighbor == gameObject) continue;
            positions += neighbor.transform.position;
            count++;
        }

        if (count == 0) return Vector3.zero;

        Vector3 center = positions / count;
        Vector3 direction = center - transform.position;

        Vector3 force = GetSteeringForce(direction);

        return force;
    }

    private Vector3 Separation(GameObject[] neighbors, float radius)
    {
        Vector3 separation = Vector3.zero;
        int count = 0;
        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor == null || neighbor == gameObject) continue;

            Vector3 direction = transform.position - neighbor.transform.position;
            float distance = direction.magnitude;
            if (distance > 0 && distance < radius)
            {
                separation += (direction.normalized / distance);
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        Vector3 force = (separation.sqrMagnitude > 0) ? GetSteeringForce(separation) : Vector3.zero;

        return force;
    }
    private Vector3 Alignment(GameObject[] neighbors)
    {
        Vector3 velocities = Vector3.zero;
        int count = 0;
        foreach (GameObject neighbor in neighbors)
        {
            if (neighbor == null) continue;
            if (neighbor.TryGetComponent<AutonomousAgent>(out var agent) && agent != this)
            {
                Vector3 v = agent.movement.Velocity;
                v.y = 0f; // ignore vertical velocity when averaging
                velocities += v;
                count++;
            }
        }
        if (count == 0) return Vector3.zero;

        Vector3 averageVelocity = velocities / count;

        Vector3 force = (averageVelocity.sqrMagnitude > 0) ? GetSteeringForce(averageVelocity) : Vector3.zero;

        return force;
    }
}
