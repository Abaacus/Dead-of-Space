using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    public float bulletSpeed;
    public float fireRate;
    float fireTime;

    protected virtual void Shoot(Orbision direction)
    {

    }

    public void FireGun(Orbision direction)
    {
        if (fireTime < 0)
        {
            Shoot(direction);
            fireTime = fireRate;
        }

        fireTime -= Time.deltaTime;
    }

    public void FireGun(Vector3 direction)
    {
        if (fireTime < 0)
        {
            Shoot(Orbision.Vector3ToOrbision(direction));
            fireTime = fireRate;
        }

        fireTime -= Time.deltaTime;
    }
}
