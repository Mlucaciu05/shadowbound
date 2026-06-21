using UnityEngine;

public interface IDamageable
{
    bool IsDead { get; }
    Transform DamageTransform { get; }
    float TakeDamage(DamageData damageData);
}
