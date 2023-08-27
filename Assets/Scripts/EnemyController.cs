using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthController))]
public class EnemyController : MonoBehaviour
{
    private Transform target;
    private Rigidbody rb;
    [SerializeField] private float velocity = 3;
    [SerializeField] private float damage = 1;
    private bool isCollided = false;
    private bool canDamage = true;
    private float damageInterval = .5f;
    private HealthController _healthController;
    private LayerMask _layerMask;
    private List<HealthController> targetHealthList = new List<HealthController>();
    
    void Start()
    {
        _healthController = GetComponent<HealthController>();
        _healthController.OnDie.AddListener(Die);
        target = GameObject.Find("player/head").transform;
        rb = GetComponent<Rigidbody>();
        _layerMask = LayerMask.GetMask("player");
        StartCoroutine(DamageOverTime());
    }

    void Update()
    {
        Vector3 dir = target.position - transform.position;
        float distance = dir.magnitude;
        dir = dir.normalized;
        if (distance > 1)
        {
            rb.velocity = dir * velocity;
            transform.forward = dir;
        }

        if (distance > 100)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((_layerMask & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log(other.gameObject.name + " enter");
            isCollided = true;
            HealthController targetHealth = other.transform.GetComponent<HealthController>();
            if (!targetHealthList.Contains(targetHealth))
            {
                targetHealthList.Add(targetHealth);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        
        if ((_layerMask & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log(other.gameObject.name + " exit");
            isCollided = false;
            HealthController targetHealth = other.transform.GetComponent<HealthController>();
            if (targetHealthList.Contains(targetHealth))
            {
                targetHealthList.Remove(targetHealth);
            }
        }
    }
    
    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            foreach (HealthController targetHealth in targetHealthList)
            {
                targetHealth.TakeDamage(damage);
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }
    
    private IEnumerator EnableDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageInterval);
        canDamage = true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}