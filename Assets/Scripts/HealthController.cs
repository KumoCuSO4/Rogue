using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50;
    private float health = 0;
    public UnityEvent OnDie;
    public bool isAlive = true;
    private Image healthFillUI = null;
    private bool _ishealthFillUINotNull;

    void Start()
    {
        health = maxHealth;
        Transform t = transform.Find("canvas/health/fill");
        if (t != null)
        {
            healthFillUI = t.GetComponent<Image>();
        }
        _ishealthFillUINotNull = healthFillUI != null;
    }

    void Update()
    {
        
    }
    
    public void TakeDamage(float damage)
    {
        // Debug.Log(gameObject.name + " " + health);
        if(!isAlive) return;
        health -= damage;
        if (_ishealthFillUINotNull)
        {
            healthFillUI.fillAmount = health / maxHealth;
        }
        if (this.health <= 0)
        {
            isAlive = false;
            OnDie.Invoke();
        }
    }
}
