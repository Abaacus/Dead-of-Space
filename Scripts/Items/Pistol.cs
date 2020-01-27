using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BaseGun
{
    protected override void Shoot(Orbision direction)
    {
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation).GetComponent<Bullet>();
        bullet.SetVelocity(direction, bulletSpeed);
    }
}
