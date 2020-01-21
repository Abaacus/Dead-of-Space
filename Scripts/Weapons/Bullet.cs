using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    private GravityBody gb;

    public float mass = 0.01f;
    public float damage;
    public float lifeSpan;

    private Orbision dir;
    private float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gb = new GravityBody(rb, mass);
        StartCoroutine(BulletDecay());
    }

    private void Update()
    {
        gb.Orbit(Vector2.up * speed);
        gb.Elevate(dir.h);
    }

    public void SetVelocity(Orbision dir, float speed)
    {
        this.dir = dir;
        this.speed = speed;
    }

    private void Hit(Collider hitCollider)
    {
        if (hitCollider != null)    // if the bullet hit something.
        {
            Debug.Log("Hit " + hitCollider);    // debug what it hit

        }
        else
        {
            Debug.Log("Hit nothing.");  // if it didn't hit anything, debug that info
        }

        GravitySource.instance.RemoveGravityObject(gb); // remove the gravitybody from the gravitybody list
        Destroy(gameObject);    // destroy this gameObject
    }

    private void OnTriggerEnter(Collider collider)  // if the bullet hit something, return what the bullet hit
    {
        Hit(collider);
    }

    IEnumerator BulletDecay()   // coroutine that destroys bullet if it's been alive for too long
    {
        yield return new WaitForSeconds(lifeSpan);  // waits for the length of time the bullet should exist for...
        Hit(null);  // return that the bullet has hit nothing
    }
}
