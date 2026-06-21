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
        if (enemyAnimator != null)
        {
            if (HasParameter("Hit", AnimatorControllerParameterType.Trigger))
            {
                enemyAnimator.SetTrigger("Hit");
            }
            else if (HasParameter("impact", AnimatorControllerParameterType.Trigger))
            {
                enemyAnimator.SetTrigger("impact");
            }
        }
    }

    private void HandleEnemyDeath()
    {
        if (enemyAnimator != null)
        {
            if (HasParameter("Die", AnimatorControllerParameterType.Trigger))
            {
                enemyAnimator.SetTrigger("Die");
            }
            else if (HasParameter("death", AnimatorControllerParameterType.Trigger))
            {
                enemyAnimator.SetTrigger("death");
            }
        }

        Destroy(gameObject, 3f);
    }

    private bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (enemyAnimator == null) return false;

        foreach (AnimatorControllerParameter parameter in enemyAnimator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}
