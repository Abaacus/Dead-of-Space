using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise3D
{
    public static float PerlinNoise(float x, float y, float z, Vector3 offset)
    {
        x += offset.x;
        y += offset.y;
        z += offset.z;

        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        float noise = xy + xz + yx + yz + zx + zy;

        return noise / 6f;
    }

    public static float PerlinNoise(Vector3 coor, Vector3 offset)
    {
        float x = coor.x + offset.x;
        float y = coor.y + offset.y;
        float z = coor.z + offset.z;

        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float yz = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        float noise = xy + xz + yx + yz + zx + zy;

        return noise / 6f;
    }
}
