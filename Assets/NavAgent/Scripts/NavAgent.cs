using UnityEngine;

public class NavAgent : AI_Agent
{
    [SerializeField] Movement movement;

    public Vector3 Destination {
        get { return movement.Destination; }
        set { movement.Destination = value; }
    }
}
