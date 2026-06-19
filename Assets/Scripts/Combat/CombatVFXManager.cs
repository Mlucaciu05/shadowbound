using UnityEngine;

public class CombatVFXManager : MonoBehaviour
{
    public static CombatVFXManager Instance { get; private set; }

    [Header("Default Effects")]
    public ParticleSystem meleeHitEffect;
    public ParticleSystem physicalHitEffect;
    public ParticleSystem fireHitEffect;
    public ParticleSystem iceHitEffect;
    public ParticleSystem arcaneHitEffect;
    public ParticleSystem deathEffect;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void Spawn(ParticleSystem prefab, Vector3 position, Vector3 normal)
    {
        if (prefab == null) return;

        Quaternion rotation = normal.sqrMagnitude > 0.001f ? Quaternion.LookRotation(normal.normalized) : Quaternion.identity;
        ParticleSystem instance = Instantiate(prefab, position, rotation);
        Destroy(instance.gameObject, GetDuration(instance));
    }

    public void SpawnHitEffect(DamageData damageData, Vector3 fallbackPosition)
    {
        ParticleSystem effect = GetHitEffect(damageData.damageType);
        Vector3 position = damageData.hitPoint == Vector3.zero ? fallbackPosition : damageData.hitPoint;
        Vector3 normal = damageData.hitDirection.sqrMagnitude > 0.001f ? -damageData.hitDirection : Vector3.up;
        Spawn(effect, position, normal);
    }

    public ParticleSystem GetHitEffect(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Fire:
                return fireHitEffect != null ? fireHitEffect : meleeHitEffect;
            case DamageType.Ice:
                return iceHitEffect != null ? iceHitEffect : meleeHitEffect;
            case DamageType.Arcane:
                return arcaneHitEffect != null ? arcaneHitEffect : meleeHitEffect;
            default:
                return physicalHitEffect != null ? physicalHitEffect : meleeHitEffect;
        }
    }

    private static float GetDuration(ParticleSystem particleSystem)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        return main.duration + main.startLifetime.constantMax + 0.25f;
    }
}
