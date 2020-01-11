using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public static GravitySource instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of " + this + " found");
        }

        instance = this;

        gravityBodies = new List<GravityBody>();
        Orbision.hOrigin = transform.position;
    }

    [SerializeField]
    List<GravityBody> gravityBodies;

    [SerializeField]
    float gravityStrength = 1f;

    public float groundHitBuffer = 0.2f;

    private void FixedUpdate()
    {
        foreach (GravityBody gravityBody in gravityBodies)
        {
            gravityBody.Attract(-gravityStrength);
        }
    }

    public void AddGravityObject(GravityBody gravityBody)
    {
        gravityBodies.Add(gravityBody);
    }
}
