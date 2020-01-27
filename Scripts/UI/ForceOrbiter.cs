using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOrbiter : MonoBehaviour
{
    public Transform rotatePoint;
    public Vector3 axisOfRotation;
    public float objSpeed;

    public Transform[] rotatingObjects;

    private void FixedUpdate()
    {
        foreach (Transform obj in rotatingObjects)
        {
            obj.RotateAround(rotatePoint.position, axisOfRotation.normalized, objSpeed);
        }
    }
}
