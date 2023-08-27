using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 30;
    public float damage = 10;
    private Vector3 dir;
    private float destroyTime = 10;
    private LayerMask _layerMask;
    
    void Start()
    {
        dir = transform.forward;
        StartCoroutine(DestroyAfterTime());
        _layerMask = LayerMask.GetMask("enemy");
    }

    void Update()
    {
        Vector3 moveDistance = dir * (speed * Time.deltaTime);
        transform.position += moveDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_layerMask & (1 << other.gameObject.layer)) != 0)
        {
            HealthController healthController = other.GetComponent<HealthController>();
            healthController.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
