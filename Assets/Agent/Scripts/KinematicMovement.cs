using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicMovement : Movement
{
    public override void ApplyForce(Vector3 force)
    {
        // Avoid accumulating vertical force
        force.y = 0f;
        Acceleration += force;
    }
    private void LateUpdate()
    {
        Velocity += Acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        // lock vertical velocity to zero and apply horizontal motion only
        Velocity = new Vector3(Velocity.x, 0f, Velocity.z);

        transform.position += new Vector3(Velocity.x, 0f, Velocity.z) * Time.deltaTime;

        Acceleration = Vector3.zero;
    }
}
