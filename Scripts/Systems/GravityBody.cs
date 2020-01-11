using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody
{
    GravitySource gravitySource;
    Transform attractingPoint;

    float groundHitBuffer;

    Transform transform;
    Orbision orbision;
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
    }

    internal void Attract(float gravityStrength)
    {
        AlignWithGravityNormal();

        if (!isWeightless)
        {
            Vector3 gravityForce = WeightedInvertedGravityNormal() * gravityStrength;
            rb.AddForce(gravityForce);
        }
    }

    void AlignWithGravityNormal()
    {
        transform.rotation = Quaternion.FromToRotation(transform.up, InvertedGravityNormal()) * transform.rotation;
    }

    Vector3 InvertedGravityNormal()
    {
        Debug.DrawLine(transform.position, attractingPoint.position, Color.black);
        return (transform.position - attractingPoint.position).normalized;
    }

    Vector3 GravityNormal()
    {
        Debug.DrawLine(transform.position, attractingPoint.position);
        return (attractingPoint.position - transform.position).normalized;
    }

    Vector3 WeightedInvertedGravityNormal()
    {
        timeStep += Time.deltaTime;
        if (OnGround())
        {
            timeStep = 1;
        }

        return InvertedGravityNormal() * mass * timeStep;
    }

    bool OnGround()
    {
        Debug.DrawRay(transform.position, GravityNormal() * 10, Color.green);
        if (Physics.Raycast(transform.position, GravityNormal(), out RaycastHit hit, 10))
        {
            return hit.distance <= groundHitBuffer;
        }
        else
        {
            return false;
        }
    }

    public float HeightFromPlanet()
    {
        return Vector3.Distance(transform.position, attractingPoint.position);
    }

    public void Boost(float jumpPower)
    {
        if (OnGround())
        {
            Debug.Log("Boosting");
            rb.AddForce(transform.up * jumpPower / mass);
        }
    }

    public void Orbit(Vector3 deltaMove)
    {
        //deltaMove.y = 0;
        moveAmount = Vector3.SmoothDamp(moveAmount, deltaMove, ref smoothMoveVelocity, .15f);

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void Orbit(Vector2 deltaMove)
    {
        Vector3 targetMoveAmount = new Vector3(deltaMove.x, 0, deltaMove.y);
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void Orbit(float deltaX, float deltaZ)
    {
        Vector3 targetMoveAmount = new Vector3(deltaX, 0, deltaZ);
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void Elevate(float deltaW)
    {
        //AlignWithGravityNormal();
        rb.MovePosition(rb.position + (InvertedGravityNormal() * deltaW * Time.fixedDeltaTime));
    }

    public void Spin(float angle)
    {
        AlignWithGravityNormal();
        transform.Rotate(Vector3.up * angle * Time.deltaTime);
    }
}