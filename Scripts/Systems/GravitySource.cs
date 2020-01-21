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
        Orbision.AddOrigin(transform.position);
    }

    [SerializeField]
    List<GravityBody> gravityBodies;

    [SerializeField]
    float gravityStrength = 1f;

    bool gameStarted;

    public float groundHitBuffer = 0.2f;

    private void FixedUpdate()
    {
        if (gameStarted)
        {
            foreach (GravityBody gb in gravityBodies)
            {
                gb.Attract(-gravityStrength);
                gb.Move();
            }
        }
    }

    public void AddGravityObject(GravityBody gravityBody)
    {
        gravityBodies.Add(gravityBody);
    }

    public void RemoveGravityObject(GravityBody gravityBody)
    {
        gravityBodies.Remove(gravityBody);
    }
}
