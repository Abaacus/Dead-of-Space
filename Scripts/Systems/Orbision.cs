using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* while unity has a vector3 system to represent positions of objects, this was insufficient for my purposes
 * 
 */

public class Orbision
{
    #region Properties

    public float h; // mechanics break if this is less than 1
    public float i;
    public float j;
    public float k;

    public Vector3 localUp
    {
        get { return new Vector3(i, j, k); }
    }

    public Vector3 localRight
    {
        get { return new Vector3(j, k, i); }
    }

    public Vector3 localForward
    {
        get { return new Vector3(k, i, j); }
    }

    public Vector3 vector3Position
    {
        get{ return localUp * h; }
    }

    public static List<Vector3> hOrigins = new List<Vector3>();
    Vector3 _hOrigin;
    public Vector3 hOrigin
    {
        get => _hOrigin;

        set
        {
            AddOrigin(value);

            if (value != hOrigin)
            {
                Vector3 rawPosition = vector3Position;
                _hOrigin = value;
                Orbision newOrbision = Vector3ToOrbision(rawPosition);

                h = newOrbision.h;
                i = newOrbision.i;
                j = newOrbision.j;
                k = newOrbision.k;
            }
        }
    }

    #endregion

    #region Static Properties

    public static Orbision up
    {
        get { return new Orbision(0, 1, 0); }
    }

    public static Orbision right
    {
        get { return new Orbision(1, 0, 0); }
    }

    public static Orbision forward
    {
        get { return new Orbision(0, 0, 1); }
    }

    #endregion

    #region Constructors

    public Orbision()
    {

    }

    public Orbision(float i, float j, float k, int originIndex = 0)
    {
        h = 0;
        this.i = i;
        this.j = j;
        this.k = k;
        hOrigin = hOrigins[originIndex];
    }

    public Orbision(float h, float i, float j, float k, int originIndex = 0) : this(i, j, k, originIndex)
    {
        this.h = h;
    }

    public Orbision(Vector3 direction, int originIndex = 0) : this(direction.x, direction.y, direction.z, originIndex)
    {
    }

    public Orbision(float h, Vector3 direction, int originIndex = 0) : this(direction, originIndex)
    {
        this.h = h;
    }

    public Orbision(Orbision orbision) : this(orbision.h, orbision.i, orbision.j, orbision.k, hOrigins.IndexOf(orbision.hOrigin))
    {

    }

    #endregion

    #region Children Methods

    public override string ToString()
    {
        return "Orbision (" + h + "|" + i + ", " + j + ", " + k + ")";
    }

    public void RotateAroundAxis(Vector3 axis, float angle)
    {
        Vector3 newDir = Quaternion.AngleAxis(angle, axis) * vector3Position;
        newDir.Normalize();
        i = newDir.x;
        j = newDir.y;
        k = newDir.z;
    }

    #endregion

    #region Static Methods

    public static float CalculateDeltaH(Vector3 axis, Vector3 deltaMove)
    {
        axis.Normalize();
        deltaMove.Normalize();

        Vector3 rotAxis = Vector3.Cross(Vector3.right, axis);
        float rotAngle = Mathf.Cos(Vector3.Dot(Vector3.right, axis));

        Vector2 rotatedPoint = Quaternion.AngleAxis(rotAngle, rotAxis) * deltaMove;
        return rotatedPoint.y;
    }

    public static Orbision Vector3ToOrbision(Vector3 vector3, int originIndex = 0)
    {
        Vector3 dirToPoint = (vector3 - hOrigins[originIndex]).normalized;

        Orbision orbision = new Orbision
        {
            h = Vector3.Distance(hOrigins[originIndex], vector3),
            i = dirToPoint.x,
            j = dirToPoint.y,
            k = dirToPoint.z
        };

        return orbision;
    }

    public static void AddOrigin(Vector3 newOrigin)
    {
        bool originAdded = false;
        foreach (Vector3 hOrigin in hOrigins)
        {
            if (hOrigin == newOrigin)
            {
                originAdded = true;
            }
        }

        if (!originAdded)
        { hOrigins.Add(newOrigin); }
    }

    #endregion

    #region Operator

    public static Orbision operator +(Orbision a, Orbision b)
    {
        Vector3 dir = new Vector3
        {
            x = a.i + b.i,
            y = a.j + b.j,
            z = a.k + b.k
        };

        float abH = a.h + b.h;

        int hSign = 1;
        if (Mathf.Abs(abH) >= Mathf.Epsilon)
        {
            hSign = (int)(abH / Mathf.Abs(abH));
        }

        Orbision ab = new Orbision(hSign * Mathf.Clamp(abH, 1, Mathf.Infinity), dir.normalized);
        return ab;
    }

    public static Orbision operator -(Orbision a, Orbision b)
    {
        Vector3 dir = new Vector3
        {
            x = a.i - b.i,
            y = a.j - b.j,
            z = a.k - b.k
        };

        float abH = a.h - b.h;

        int hSign = 1;
        if (Mathf.Abs(abH) >= Mathf.Epsilon)
        {
            hSign = (int)(abH / Mathf.Abs(abH));
        }

        Orbision ab = new Orbision(hSign * Mathf.Clamp(abH, 1, Mathf.Infinity), dir.normalized);
        return ab;
    }

    #endregion
}
