using UnityEngine;
using UnityEngine.Events;

public class GenericHealth : MonoBehaviour, IDamageable
{
    [Header("Generic Health")]
    public float currentHealth = 100f;
    public int maxHealth = 100;
    public bool isDead = false;

    [Header("Defense")]
    public float defense = 0f;
    public bool useDefenseReduction = true;

    [Header("Feedback")]
    public Animator animator;
    public Rigidbody knockbackBody;
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;
    public string hitTrigger = "Hit";
    public string legacyHitTrigger = "impact";
    public string deathTrigger = "Die";
    public string legacyDeathTrigger = "death";
    public bool destroyOnDeath = false;
    public float destroyDelay = 3f;

    [Header("Events")]
    public UnityEvent<float> onTakeDamage;
    public UnityEvent onDeath;

    public bool IsDead
    {
        get { return isDead; }
    }

    public Transform DamageTransform
    {
        get { return transform; }
    }

    void Awake()
    {
        if (maxHealth <= 0)
        {
            maxHealth = Mathf.Max(1, Mathf.RoundToInt(currentHealth));
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (knockbackBody == null)
        {
            knockbackBody = GetComponent<Rigidbody>();
        }
    }

    void Start()
    {
        if (currentHealth <= 0f || currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        onTakeDamage?.Invoke(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        TakeDamage(new DamageData(damage, DamageType.Physical, null, 0f, false));
    }

    public float TakeDamage(DamageData damageData)
    {
        if (isDead) { return 0f; }

        float finalDamage = CalculateFinalDamage(damageData);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onTakeDamage?.Invoke(currentHealth);
        PlayHitFeedback(damageData);
        ApplyKnockback(damageData);

        if (currentHealth <= 0)
        {
            Die();
        }

        return finalDamage;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        SetAnimatorTrigger(deathTrigger, legacyDeathTrigger);
        SpawnEffect(deathEffect);
        onDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        onTakeDamage?.Invoke(currentHealth);
    }

    public float CalculateFinalDamage(DamageData damageData)
    {
        float incomingDamage = Mathf.Max(0f, damageData.damageAmount);

        PlayerStats playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            return playerStats.CalculateIncomingDamage(damageData, incomingDamage);
        }

        if (!useDefenseReduction)
        {
            return incomingDamage;
        }

        return ApplyDefenseReduction(incomingDamage, defense);
    }

    public static float ApplyDefenseReduction(float incomingDamage, float defenseAmount)
    {
        defenseAmount = Mathf.Max(0f, defenseAmount);
        return incomingDamage * (100f / (100f + defenseAmount));
    }

    private void PlayHitFeedback(DamageData damageData)
    {
        SetAnimatorTrigger(hitTrigger, legacyHitTrigger);

        if (hitEffect != null)
        {
            SpawnEffect(hitEffect);
        }
        else if (CombatVFXManager.Instance != null)
        {
            CombatVFXManager.Instance.SpawnHitEffect(damageData, transform.position + Vector3.up);
        }
    }

    private void ApplyKnockback(DamageData damageData)
    {
        if (knockbackBody == null || damageData.knockbackForce <= 0f) return;

        Vector3 direction = damageData.hitDirection;
        if (direction.sqrMagnitude < 0.001f && damageData.attacker != null)
        {
            direction = transform.position - damageData.attacker.transform.position;
        }

        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        knockbackBody.AddForce(direction.normalized * damageData.knockbackForce, ForceMode.Impulse);
    }

    private void SpawnEffect(ParticleSystem effect)
    {
        if (effect == null) return;

        ParticleSystem instance = Instantiate(effect, transform.position + Vector3.up, Quaternion.identity);
        ParticleSystem.MainModule main = instance.main;
        Destroy(instance.gameObject, main.duration + main.startLifetime.constantMax + 0.25f);
    }

    private void SetAnimatorTrigger(string primaryTrigger, string fallbackTrigger)
    {
        if (animator == null) return;

        if (HasAnimatorParameter(primaryTrigger, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(primaryTrigger);
            return;
        }

        if (HasAnimatorParameter(fallbackTrigger, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(fallbackTrigger);
        }
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (string.IsNullOrEmpty(parameterName) || animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}
