using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuttle : MonoBehaviour
{
    Rigidbody rb;
    GravityBody gb;

    public float speed;
     
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gb = new GravityBody(rb, SpinType.axis);
    }

    private void Update()
    {
        gb.Orbit(Vector2.up * Time.fixedDeltaTime * speed);
    }
}
