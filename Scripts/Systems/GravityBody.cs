using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody
{
    GravitySource gravitySource;
    Transform attractingPoint;

    float groundHitBuffer;

    Transform transform;
    public Orbision orbision;
    public Rigidbody rb;
    float mass;
    float timeStep = 1;
    bool isWeightless = false;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    public GravityBody(Rigidbody rb, float mass = 1)
    {
        gravitySource = GravitySource.instance;

        transform = rb.transform;
        this.rb = rb;
        rb.useGravity = false;
        rb.freezeRotation = true;

        this.mass = mass;
        isWeightless = false;

        groundHitBuffer = gravitySource.groundHitBuffer;
        attractingPoint = gravitySource.transform;
        gravitySource.AddGravityObject(this);

        orbision = Orbision.Vector3ToOrbision(transform.position);
    }

    public GravityBody(Rigidbody rb)
    {
        gravitySource = GravitySource.instance;

        transform = rb.transform;
        this.rb = rb;
        rb.useGravity = false;
        rb.freezeRotation = true;

        mass = 1;
        isWeightless = true;

        groundHitBuffer = gravitySource.groundHitBuffer;
        attractingPoint = gravitySource.transform;
        gravitySource.AddGravityObject(this);

        orbision = Orbision.Vector3ToOrbision(transform.position);
    }

    internal void Attract(float gravityStrength)
    {
        AlignWithGravityNormal();

        if (!isWeightless)
        {
            Vector3 gravityForce = WeightedGravityNormal() * gravityStrength;
            rb.AddForce(gravityForce);
        }
    }

    void AlignWithGravityNormal()
    {
        transform.rotation = Quaternion.FromToRotation(transform.up, orbision.up) * transform.rotation;
    }

    Vector3 WeightedGravityNormal()
    {
        timeStep += Time.deltaTime;
        if (OnGround())
        {
            timeStep = 1;
        }

        return orbision.up * mass * timeStep;
    }

    bool OnGround()
    {
        Debug.DrawRay(transform.position, orbision.up * 10, Color.green);
        Debug.DrawRay(transform.position, orbision.right * 10, Color.red);
        Debug.DrawRay(transform.position, orbision.forward * 10, Color.blue);
        if (Physics.Raycast(transform.position, -orbision.up, out RaycastHit hit, 10))
        {
            return hit.distance <= groundHitBuffer;
        }
        else
        {
            return false;
        }
    }

    public void Boost(float jumpPower)
    {
        if (OnGround())
        {
            Debug.Log("Boosting");
            rb.AddForce(transform.up * jumpPower / mass);
            orbision = Orbision.Vector3ToOrbision(rb.position);
        }
    }

    public void Orbit(Vector2 deltaMove)
    {
        AlignWithGravityNormal();
        Orbision newOrbision = new Orbision(orbision);
        newOrbision.RotateAroundAxis(-transform.forward, deltaMove.x);
        newOrbision.RotateAroundAxis(transform.right, deltaMove.y);
        newOrbision -= orbision;

        rb.MovePosition(rb.position + newOrbision.vector3Position);
        orbision = Orbision.Vector3ToOrbision(rb.position);
    }

    public void Elevate(float deltaH)
    {
        AlignWithGravityNormal();

        rb.MovePosition(rb.position + orbision.up * deltaH * Time.fixedDeltaTime);
        orbision = Orbision.Vector3ToOrbision(rb.position);
    }

    public void Spin(float angle)
    {
        AlignWithGravityNormal();
        transform.RotateAround(orbision.hOrigin, orbision.up, angle * Time.deltaTime);
    }

    public void Move(Orbision orbision)
    {
        Elevate(orbision.h);
        Orbit(orbision.vector3Position - this.orbision.vector3Position);
    }
}