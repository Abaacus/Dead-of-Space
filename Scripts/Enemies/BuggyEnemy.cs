using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyEnemy : BaseEnemy
{
    public float minHeight;
    static float heightOffset = 5f;
    float targetHeight;

    public float boundsRadius = 8f;
    public float veerStrength = 10f;

    new void Start()
    {
        base.Start();
        gb = new GravityBody(GetComponent<Rigidbody>());
        targetHeight = Random.Range(planetData.coreRadius + minHeight, planetData.planetRadius + heightOffset);
    }

    public override void Wander()
    {
        if (Mathf.Abs(targetHeight - gb.orbision.h) <= 0.2f)
        {
            targetHeight = Random.Range(planetData.coreRadius + heightOffset, planetData.planetRadius + heightOffset);
        }

        float deltaY = (targetHeight - gb.orbision.h) * verticalSpeed;
        if (deltaY <= 2f)
        {
            deltaY += BobNoise(Time.fixedTime);
        }
        //gravityBody.Spin(BobNoise(2 * Time.fixedTime) * rotSpeed);

        //gravityBody.Elevate(deltaY);
        //gravityBody.Orbit(0, moveSpeed);

        if (true)//IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Debug.DrawRay(transform.position, collisionAvoidDir, Color.red);
            Vector3 planetDirection = -gb.orbision.up;
            Debug.DrawRay(transform.position, new Vector2(planetDirection.x, planetDirection.z), Color.green);

            //gravityBody.Elevate(planetDirection.y);
            //gravityBody.Spin()

            //planetDirection 
        }

        
    }

    public override void Target()
    {

    }

    public override void Dead()
    {

        base.Dead();
    }

    float BobNoise(float fixedX)
    {
        float noise = Mathf.Sin(fixedX);
        noise += Mathf.Sin(2 * fixedX - 1);
        return noise;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = SphereDirections.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, boundsRadius))
            {
                return dir;
            }
        }

        return transform.forward;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, boundsRadius, transform.forward, out hit))
        {
            return true;
        }
        else
        { }
        return false;
    }

    new void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Vector3.zero, minHeight + planetData.coreRadius);
    }
}

public static class SphereDirections
{
    const int numViewDirections = 300;
    public static readonly Vector3[] directions;

    static SphereDirections()
    {
        directions = new Vector3[numViewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numViewDirections; i++)
        {
            float t = (float)i / numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
        }
    }
}

