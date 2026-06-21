using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public SpellData spellData;
    public GameObject caster;
    public PlayerStats casterStats;
    public LayerMask hitLayers = ~0;
    public float fallbackSpeed = 16f;
    public float lifetime = 4f;

    private Rigidbody projectileBody;
    private Vector3 direction;
    private bool initialized;

    void Awake()
    {
        projectileBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (!initialized)
        {
            Initialize(spellData, caster, casterStats, transform.forward, hitLayers);
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (projectileBody == null)
        {
            transform.position += direction * GetSpeed() * Time.deltaTime;
        }
    }

    public void Initialize(SpellData data, GameObject owner, PlayerStats stats, Vector3 travelDirection, LayerMask layers)
    {
        spellData = data;
        caster = owner;
        casterStats = stats;
        hitLayers = layers;
        direction = travelDirection.sqrMagnitude > 0.001f ? travelDirection.normalized : transform.forward;
        initialized = true;

        if (projectileBody == null)
        {
            projectileBody = GetComponent<Rigidbody>();
        }

        if (projectileBody != null)
        {
            projectileBody.velocity = direction * GetSpeed();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (caster != null && other.transform.IsChildOf(caster.transform)) return;
        if ((hitLayers.value & (1 << other.gameObject.layer)) == 0) return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 hitDirection = damageable.DamageTransform.position - transform.position;
        DamageData damageData = casterStats != null
            ? casterStats.BuildSpellDamage(spellData, caster, hitPoint, hitDirection)
            : new DamageData(spellData != null ? spellData.damage : 10f, spellData != null ? spellData.damageType : DamageType.Arcane, caster, spellData != null ? spellData.knockbackForce : 0f, false).WithHit(hitPoint, hitDirection);

        damageable.TakeDamage(damageData);
        ApplySlowIfNeeded(damageable);

        if (spellData != null && spellData.impactEffect != null)
        {
            CombatVFXManager.Spawn(spellData.impactEffect, hitPoint, -direction);
        }

        Destroy(gameObject);
    }

    private void ApplySlowIfNeeded(IDamageable damageable)
    {
        if (spellData == null || spellData.damageType != DamageType.Ice) return;

        EnemyController enemy = damageable.DamageTransform.GetComponent<EnemyController>();
        if (enemy == null)
        {
            enemy = damageable.DamageTransform.GetComponentInParent<EnemyController>();
        }

        if (enemy != null)
        {
            enemy.ApplySlow(spellData.slowPercent, spellData.slowDuration);
        }
    }

    private float GetSpeed()
    {
        return spellData != null ? spellData.projectileSpeed : fallbackSpeed;
    }
}
