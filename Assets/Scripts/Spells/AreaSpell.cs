using System.Collections.Generic;
using UnityEngine;

public class AreaSpell : MonoBehaviour
{
    public SpellData spellData;
    public GameObject caster;
    public PlayerStats casterStats;
    public LayerMask hitLayers = ~0;
    public bool applyOnStart = true;
    public float destroyDelay = 2f;

    private bool applied;

    void Start()
    {
        if (applyOnStart && !applied)
        {
            Apply();
        }
    }

    public void Initialize(SpellData data, GameObject owner, PlayerStats stats, LayerMask layers)
    {
        spellData = data;
        caster = owner;
        casterStats = stats;
        hitLayers = layers;
        Apply();
    }

    public void Apply()
    {
        if (applied) return;
        applied = true;

        float radius = spellData != null ? spellData.radius : 3f;

        if (spellData != null && spellData.areaEffect != null)
        {
            CombatVFXManager.Spawn(spellData.areaEffect, transform.position, Vector3.up);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, hitLayers, QueryTriggerInteraction.Ignore);
        List<IDamageable> damagedTargets = new List<IDamageable>();

        foreach (Collider hit in hits)
        {
            if (caster != null && hit.transform.IsChildOf(caster.transform)) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead || damagedTargets.Contains(damageable)) continue;

            damagedTargets.Add(damageable);
            Vector3 hitDirection = damageable.DamageTransform.position - transform.position;
            DamageData damageData = casterStats != null
                ? casterStats.BuildSpellDamage(spellData, caster, damageable.DamageTransform.position, hitDirection)
                : new DamageData(spellData != null ? spellData.damage : 20f, spellData != null ? spellData.damageType : DamageType.Arcane, caster, spellData != null ? spellData.knockbackForce : 0f, false).WithHit(damageable.DamageTransform.position, hitDirection);

            damageable.TakeDamage(damageData);
        }

        Destroy(gameObject, destroyDelay);
    }
}
