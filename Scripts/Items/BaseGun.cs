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

    private void Update()
    {
        fireTime -= Time.deltaTime;
    }

    protected virtual void Shoot(Orbision direction)
    {

    }

    public void FireGun(Vector3 direction)
    {
        if (fireTime < 0)
        {
            Orbision oDirection = new Orbision
            {
                i = direction.x,
                j = direction.y,
                k = direction.z,
                h = Orbision.CalculateDeltaH(transform.forward, direction)
            };

            Shoot(oDirection);
            fireTime = fireRate;
        }
    }
}
