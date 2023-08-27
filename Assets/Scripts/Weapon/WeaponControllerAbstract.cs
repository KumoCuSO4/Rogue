using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Weapon
{
    public abstract class WeaponControllerAbstract : MonoBehaviour
    {
        [SerializeField] protected float attackRange = 20;
        [SerializeField] protected float damage = 10;
        protected bool hasTarget = false;
        protected bool canShoot = true;
        [SerializeField] protected float shootInterval = .5f;
        protected GameObject bulletPrefab;
        [SerializeField] protected Transform bulletSpawnPoint;
        
        
        void Start()
        {
            bulletPrefab = Resources.Load("Prefabs/bullet") as GameObject;
            StartCoroutine(ShootCoroutine());
        }

        void Update()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

            float closestDistance = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<EnemyController>())
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = collider.gameObject;
                    }
                }
            }

            if (closestObject)
            {
                hasTarget = true;
                transform.LookAt(closestObject.transform);
            }
            else
            {
                hasTarget = false;
            }
        }
        
        private IEnumerator ShootCoroutine()
        {
            while (true)
            {
                if (canShoot && hasTarget)
                {
                    yield return Attack();
                    StartCoroutine(EnableShootAfterDelay());
                }
                yield return null;
            }
        }

        protected abstract IEnumerator Attack();

        private IEnumerator EnableShootAfterDelay()
        {
            yield return new WaitForSeconds(shootInterval);
            canShoot = true;
        }
    }
}

