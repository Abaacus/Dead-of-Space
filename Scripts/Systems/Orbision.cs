using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbision
{
    public float h;
    public float i;
    public float j;
    public float k;

    public Vector3 direction
    {
        get { return new Vector3(i, j, k); }
    }

    public static Vector3 hOrigin
    {
        get { return hOrigin; }

        set
        {
            if (value != hOrigin)
            {
                Vector3 rawPos = OrbisionToVector3();
                hOrigin = value;
                this = Vector3ToOrbision(rawPos);
            }
        }
    }

    public Orbision()
    {

    }

    public Orbision(float i, float j, float k)
    {
        this.i = i;
        this.j = j;
        this.k = k;
    }

    public Orbision(float h, float i, float j, float k) : this(i, j, k)
    {
        this.h = h;
    }

    #region Static Methods

    public static Orbision Vector3ToOrbision(Vector3 vector3)
    {
        Vector3 dirToPoint = (vector3 - hOrigin).normalized;

        Orbision orbision = new Orbision
        {
            h = Vector3.Distance(hOrigin, vector3),
            i = dirToPoint.x,
            j = dirToPoint.y,
            k = dirToPoint.z
        };

        return orbision;
    }

    public static Vector3 OrbisionToVector3()
    {
        return new Vector3(i, j, k) * h;
    }

    void RecalculateOrbisionConstants()
    {
        direction = new Vector3(i, j, k);
    }
    
    #endregion

    #region Operator

    public static Orbision operator +(Orbision a, Orbision b)
    {
        return new Orbision
        {
            h = a.h + b.h,
            i = a.i + b.i,
            j = a.j + b.j,
            k = a.k + b.k,
        };
    }
    #endregion
}
