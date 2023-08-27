using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthController))]
public class HeadController : MonoBehaviour
{
    private float velocity = 5f;
    private float rotateSpeed = 1f;
    private Rigidbody rb;

    private float health = 0;
    [SerializeField] private float maxHealth = 100f;
    private HealthController _healthController;
    private int bodyNum = 5;
    private GameObject _bodyObj;
    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        _healthController = GetComponent<HealthController>();
        _healthController.OnDie.AddListener(Die);
        _bodyObj = Resources.Load("Prefabs/body") as GameObject;
        Transform prevBody = transform;
        for (int i = 0; i < bodyNum; i++)
        {
            Transform newBody = Instantiate(_bodyObj, transform).transform;
            newBody.parent = transform.parent;
            newBody.position = prevBody.position - prevBody.forward * 1;
            newBody.forward = prevBody.forward;
            BodyController bodyController = newBody.GetComponent<BodyController>();
            bodyController.target = prevBody.Find("tail");
            prevBody = newBody;
        }
    }

    void Update()
    {
        int x = 0;
        int z = 0;

        if (Input.GetKey(KeyCode.W))
        {
            z++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            z--;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x++;
        }
        if(x!=0 || z!=0)
        {
            float targetDir = 90 - Mathf.Atan2(z, x) * Mathf.Rad2Deg;
            if (targetDir > 180) targetDir -= 360;
            //Debug.Log(targetDir);
            float curDir = transform.eulerAngles.y;
            float dirChange = targetDir - curDir;
            if (dirChange < -180) dirChange += 360;
            if (dirChange > 180) dirChange -= 360;
            if (Mathf.Abs(dirChange) > 1)
            {
                transform.Rotate(Vector3.up, dirChange * Time.deltaTime);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, targetDir, 0);
            }
        }
        rb.velocity = transform.forward * velocity;
    }

    private void Die()
    {
        Debug.Log("Player die");
    }
}
