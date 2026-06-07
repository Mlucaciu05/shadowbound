using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericHealth : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Generic Health")]
    public float currentHealth = 100f;
    public int maxHealth;
    public bool isDead = false;

    [Header("Events")]
    public UnityEvent<float> onTakeDamage;
    public UnityEvent onDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (isDead) { return; }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onTakeDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        onDeath?.Invoke();
    }
}
