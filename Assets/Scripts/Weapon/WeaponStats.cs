using UnityEngine;

namespace Weapon
{
    public class WeaponStats
    {
        public float attackRange = 20;
        public float damage = 10;
        public bool hasTarget = false;
        public bool canShoot = true;
        public float shootInterval = .5f;
        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;

        public WeaponStats(int weaponID)
        {
            LoadData(weaponID);
        }

        private void LoadData(int weaponID)
        {
            
        }
    }
}