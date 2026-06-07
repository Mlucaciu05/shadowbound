using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GenericHealth generalHealth;
    private Animator enemyAnimator; // Assumes your enemy has an Animator

    void Start()
    {
        generalHealth = GetComponent<GenericHealth>();
        enemyAnimator = GetComponentInChildren<Animator>();

        // Link into the core health events via code
        generalHealth.onDeath.AddListener(HandleEnemyDeath);
        generalHealth.onTakeDamage.AddListener(HandleEnemyFlinch);
    }

    public void HandleEnemyFlinch(float currentHealth)
    {
        if (enemyAnimator != null) enemyAnimator.SetTrigger("impact");
    }

    private void HandleEnemyDeath()
    {
        if (enemyAnimator != null) enemyAnimator.SetTrigger("death");

        Destroy(gameObject, 3f);
    }
}