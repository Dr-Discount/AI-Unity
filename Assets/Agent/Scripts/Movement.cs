using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public virtual Vector3 Velocity { get; set; }
    public virtual Vector3 Acceleration { get; set; }
}
