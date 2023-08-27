using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Weapon
{
    public class WeaponController : WeaponControllerAbstract
    {
        protected override IEnumerator Attack()
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.damage = damage;
            canShoot = false;
            yield return null;
        }
    }
}

