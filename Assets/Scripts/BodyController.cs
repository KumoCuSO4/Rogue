using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthController))]
public class BodyController : MonoBehaviour
{
    public Transform target;
    private float maxDistance = 0.5f;
    private Transform front;
    private Rigidbody rb;
    public float velocity = 5f;
    private HealthController _healthController;
    private GameObject _weaponObj;
    
    void Start()
    {
        front = transform.Find("front");
        rb = GetComponent<Rigidbody>();
        _healthController = GetComponent<HealthController>();
        _healthController.OnDie.AddListener(Die);
        _weaponObj = transform.Find("weapon").gameObject;
    }

    void Update()
    {
        transform.LookAt(target);
        float distance = (target.position - front.position).magnitude;
        if(distance>maxDistance)
        {
            rb.velocity = (target.position - front.position) * velocity;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    
    private void Die()
    {
        Debug.Log(gameObject.name + " die");
        Destroy(_weaponObj);
    }
}
