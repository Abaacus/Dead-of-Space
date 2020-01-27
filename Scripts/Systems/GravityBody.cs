using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpinType { point, axis}

public class GravityBody
{
    SpinType spinType;
    GravitySource gravitySource;
    Transform attractingPoint;

    float groundHitBuffer;

    Transform transform;
    public Orbision orbision;
    public Rigidbody rb;
    float mass;
    float timeStep = 1;
    bool isWeightless = false;

    float elevateAmount;
    float smoothElevateVelocity;
    const float elevateNeutralizer = -0.01f;

    float spinAngle;
    float smoothSpinVelocity;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    public GravityBody(Rigidbody rb, SpinType spinType = SpinType.axis, float mass = 1)
    {
        gravitySource = GravitySource.instance;

        transform = rb.transform;
        this.rb = rb;
        rb.useGravity = false;
        rb.freezeRotation = true;

        this.spinType = spinType;
        this.mass = mass;
        isWeightless = false;

        groundHitBuffer = gravitySource.groundHitBuffer;
        attractingPoint = gravitySource.transform;
        gravitySource.AddGravityObject(this);

        orbision = Orbision.Vector3ToOrbision(transform.position);
    }

    public GravityBody(Rigidbody rb, SpinType spinType = SpinType.axis)
    {
        gravitySource = GravitySource.instance;

        transform = rb.transform;
        this.rb = rb;
        rb.useGravity = false;
        rb.freezeRotation = true;

        this.spinType = spinType;
        mass = 1;
        isWeightless = true;

        groundHitBuffer = gravitySource.groundHitBuffer;
        attractingPoint = gravitySource.transform;
        gravitySource.AddGravityObject(this);

        orbision = Orbision.Vector3ToOrbision(rb.position);
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
        transform.rotation = Quaternion.FromToRotation(transform.up, orbision.localUp) * transform.rotation;

        /*Debug.DrawRay(transform.position, orbision.localUp * 10, Color.green);
        Debug.DrawRay(transform.position, orbision.localRight * 10, Color.red);
        Debug.DrawRay(transform.position, orbision.localForward * 10, Color.blue);*/
    }

    Vector3 WeightedGravityNormal()
    {
        timeStep += Time.deltaTime;
        if (OnGround())
        {
            timeStep = 1;
        }

        return orbision.localUp * mass * timeStep;
    }

    bool OnGround()
    {
        if (Physics.Raycast(transform.position, -orbision.localUp, out RaycastHit hit, 10))
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
        if (OnGround() && !isWeightless)
        {
            Debug.Log("Boosting");
            AlignWithGravityNormal();
            rb.AddForce(transform.up * jumpPower / mass);
        }
    }

    public void Orbit(Vector2 deltaMove, float dampeningSpeed = 1)
    {
        float speed = deltaMove.magnitude;
        deltaMove *= orbision.h;
        moveAmount = Vector3.SmoothDamp(moveAmount, new Vector3(deltaMove.x, speed * elevateNeutralizer, deltaMove.y), ref smoothMoveVelocity, dampeningSpeed * Time.deltaTime);
    }

    public void Elevate(float deltaH, float dampeningSpeed = 1)
    {
        elevateAmount = Mathf.SmoothDamp(elevateAmount, deltaH, ref smoothElevateVelocity, dampeningSpeed * Time.deltaTime);
    }

    public void Spin(float angle, float dampeningSpeed = 1)
    {
        spinAngle = Mathf.SmoothDamp(spinAngle, angle, ref smoothSpinVelocity, dampeningSpeed * Time.deltaTime);
    }

    public void MoveBody()
    {
        AlignWithGravityNormal();
        if (spinType == SpinType.axis)
        {
            transform.Rotate(orbision.localUp, spinAngle * Time.deltaTime);
        }
        else if (spinType == SpinType.point)
        {
            transform.RotateAround(orbision.hOrigin, orbision.localUp, spinAngle * Time.deltaTime);
        }

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount + (elevateAmount * orbision.localUp)) * Time.fixedDeltaTime);
        orbision = Orbision.Vector3ToOrbision(rb.position);
    }
}