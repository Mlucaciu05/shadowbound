using System;
using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour
{
    public Transform castPoint;
    public SpellData fireballSpell;
    public SpellData iceSpikeSpell;
    public SpellData arcaneBurstSpell;
    public KeyCode fireballKey = KeyCode.Alpha1;
    public KeyCode iceSpikeKey = KeyCode.Alpha2;
    public KeyCode arcaneBurstKey = KeyCode.Alpha3;
    public LayerMask hitLayers = ~0;
    public bool allowCastingWhileBlocking = false;

    private PlayerStats playerStats;
    private PlayerCombat playerCombat;
    private PlayerAnimHandler animHandler;
    private PlayerAnimationController animationController;
    private SpellData[] runtimeFallbackSpells;
    private readonly float[] nextCastTimes = new float[3];

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        playerCombat = GetComponent<PlayerCombat>();
        animHandler = GetComponent<PlayerAnimHandler>();
        animationController = GetComponent<PlayerAnimationController>();

        if (castPoint == null)
        {
            castPoint = transform;
        }

        runtimeFallbackSpells = new SpellData[3];
    }

    void Update()
    {
        if (Input.GetKeyDown(fireballKey)) TryCast(0);
        if (Input.GetKeyDown(iceSpikeKey)) TryCast(1);
        if (Input.GetKeyDown(arcaneBurstKey)) TryCast(2);
    }

    public bool TryCast(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 2) return false;
        if (Time.time < nextCastTimes[slotIndex]) return false;
        if (playerCombat != null && playerCombat.isAttacking) return false;
        if (!allowCastingWhileBlocking && playerCombat != null && playerCombat.isBlocking) return false;

        SpellData spell = GetSpell(slotIndex);
        if (spell == null) return false;

        nextCastTimes[slotIndex] = Time.time + spell.cooldown;
        TriggerCastAnimation();

        if (spell.castEffect != null)
        {
            CombatVFXManager.Spawn(spell.castEffect, castPoint.position, castPoint.forward);
        }

        if (spell.castMode == SpellCastMode.AreaAroundCaster)
        {
            CastAreaSpell(spell);
        }
        else
        {
            CastProjectileSpell(spell);
        }

        return true;
    }

    private SpellData GetSpell(int slotIndex)
    {
        if (slotIndex == 0 && fireballSpell != null) return fireballSpell;
        if (slotIndex == 1 && iceSpikeSpell != null) return iceSpikeSpell;
        if (slotIndex == 2 && arcaneBurstSpell != null) return arcaneBurstSpell;

        if (runtimeFallbackSpells[slotIndex] == null)
        {
            runtimeFallbackSpells[slotIndex] = CreateFallbackSpell(slotIndex);
        }

        return runtimeFallbackSpells[slotIndex];
    }

    private SpellData CreateFallbackSpell(int slotIndex)
    {
        SpellData spell = ScriptableObject.CreateInstance<SpellData>();
        spell.hideFlags = HideFlags.HideAndDontSave;

        if (slotIndex == 0)
        {
            spell.spellName = "Fireball";
            spell.damageType = DamageType.Fire;
            spell.castMode = SpellCastMode.Projectile;
            spell.damage = 28f;
            spell.cooldown = 1.25f;
            spell.range = 18f;
            spell.projectileSpeed = 18f;
            spell.knockbackForce = 3f;
        }
        else if (slotIndex == 1)
        {
            spell.spellName = "Ice Spike";
            spell.damageType = DamageType.Ice;
            spell.castMode = SpellCastMode.Projectile;
            spell.damage = 22f;
            spell.cooldown = 1.5f;
            spell.range = 16f;
            spell.projectileSpeed = 20f;
            spell.slowPercent = 0.35f;
            spell.slowDuration = 2f;
        }
        else
        {
            spell.spellName = "Arcane Burst";
            spell.damageType = DamageType.Arcane;
            spell.castMode = SpellCastMode.AreaAroundCaster;
            spell.damage = 32f;
            spell.cooldown = 4f;
            spell.radius = 4f;
            spell.knockbackForce = 5f;
        }

        return spell;
    }

    private void CastProjectileSpell(SpellData spell)
    {
        Vector3 origin = castPoint.position;
        Vector3 direction = castPoint.forward;

        if (spell.projectilePrefab != null)
        {
            GameObject projectileObject = Instantiate(spell.projectilePrefab, origin, Quaternion.LookRotation(direction));
            SpellProjectile projectile = projectileObject.GetComponent<SpellProjectile>();
            if (projectile == null)
            {
                projectile = projectileObject.AddComponent<SpellProjectile>();
            }

            projectile.Initialize(spell, gameObject, playerStats, direction, hitLayers);
            return;
        }

        CastFallbackRaySpell(spell, origin, direction);
    }

    private void CastFallbackRaySpell(SpellData spell, Vector3 origin, Vector3 direction)
    {
        RaycastHit[] hits = Physics.SphereCastAll(origin, 0.3f, direction, spell.range, hitLayers, QueryTriggerInteraction.Ignore);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || hit.collider.transform.IsChildOf(transform)) continue;

            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead) continue;

            Vector3 hitDirection = damageable.DamageTransform.position - origin;
            DamageData damageData = playerStats != null
                ? playerStats.BuildSpellDamage(spell, gameObject, hit.point, hitDirection)
                : new DamageData(spell.damage, spell.damageType, gameObject, spell.knockbackForce, false).WithHit(hit.point, hitDirection);

            damageable.TakeDamage(damageData);

            if (spell.impactEffect != null)
            {
                CombatVFXManager.Spawn(spell.impactEffect, hit.point, hit.normal);
            }

            EnemyController enemy = damageable.DamageTransform.GetComponentInParent<EnemyController>();
            if (enemy != null && spell.damageType == DamageType.Ice)
            {
                enemy.ApplySlow(spell.slowPercent, spell.slowDuration);
            }

            return;
        }
    }

    private void CastAreaSpell(SpellData spell)
    {
        GameObject areaObject = new GameObject(spell.spellName + " Area");
        areaObject.transform.position = transform.position;
        AreaSpell areaSpell = areaObject.AddComponent<AreaSpell>();
        areaSpell.Initialize(spell, gameObject, playerStats, hitLayers);
    }

    private void TriggerCastAnimation()
    {
        if (animHandler != null)
        {
            animHandler.SetAnimationState("spell-cast");
        }

        if (animationController != null)
        {
            animationController.TriggerSpellCast();
        }
    }
}
