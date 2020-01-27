using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyEnemy : BaseEnemy
{
    public float minHeight;
    public float heightOffset;
    public float bobStrength;
    float targetHeight;

    public float boundsRadius = 8f;
    public float veerStrength = 10f;

    [Range(0, 1)]
    public float dropChance;
    public GameObject crystal;

    new void Start()
    {
        base.Start();
        gb = new GravityBody(GetComponent<Rigidbody>(), SpinType.axis);
        targetHeight = Random.Range(terrainPlacer.coreRadius + minHeight, terrainPlacer.planetRadius + heightOffset);
    }

    public override void Wander()
    {
        /*if (Mathf.Abs(targetHeight - gb.orbision.h) <= 0.2f)
        {
            targetHeight = Random.Range(terrainPlacer.coreRadius + heightOffset, terrainPlacer.planetRadius + heightOffset);
        }

        float deltaY = (targetHeight - gb.orbision.h) * rateOfAscension;

        gb.Elevate(deltaY, moveSpeed);
        gb.Orbit(Vector2.up * moveSpeed * Time.fixedDeltaTime);
        gb.Spin(BobNoise(Time.fixedTime, 10) * rotSpeed, moveSpeed);

        if (IsHeadingForCollision())
        {
            Debug.Log("Collision Course");
            Vector3 collisionAvoidDir = ObstacleRays();

            //gravityBody.Elevate(planetDirection.y);
            //gravityBody.Spin()

            //planetDirection 
        }*/
    }

    public override void Target()
    {

    }

    public override void Dead()
    {
        if (Random.Range(0, 1f) < dropChance)
        {
            Instantiate(crystal, transform.position, transform.rotation);
        }

        base.Dead();
    }

    float BobNoise(float x, float strength = 1, float scale = 1)
    {
        float noise = Mathf.Sin(scale * x);
        return strength * noise;
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
        Gizmos.DrawWireSphere(transform.position + (transform.forward * 2), boundsRadius);
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

