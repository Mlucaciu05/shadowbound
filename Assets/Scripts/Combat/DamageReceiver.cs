using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    public GenericHealth targetHealth;
    public Transform damageTransform;

    public bool IsDead
    {
        get { return targetHealth != null && targetHealth.IsDead; }
    }

    public Transform DamageTransform
    {
        get
        {
            if (damageTransform != null) return damageTransform;
            if (targetHealth != null) return targetHealth.transform;
            return transform;
        }
    }

    void Awake()
    {
        if (targetHealth == null)
        {
            targetHealth = GetComponentInParent<GenericHealth>();
        }
    }

    public float TakeDamage(DamageData damageData)
    {
        if (targetHealth == null) return 0f;
        return targetHealth.TakeDamage(damageData);
    }
}
