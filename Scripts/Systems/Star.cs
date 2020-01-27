using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour   // controls the star the planet orbits around (surprise! the star moves, not the planet)
{
    private Rigidbody rb;   // physics used in moving star  (it could potentially crash into the planet)
    private GravityBody gb; // gravityBody (used to orbit the star around the center)

    private Vector2 direction;  // random direction that the star orbits in
    public float speed; // speed of the star's orbit

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // load the rigidbody
        gb = new GravityBody(rb, SpinType.axis);   // creates a new gravity body
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;   // creates a random circle for the star to orbit around the planet
    }

    private void Update()
    {
        gb.Orbit(direction * Time.fixedDeltaTime * speed);  // orbits the star around the center
    }
}
