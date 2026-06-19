using UnityEngine;

public class BossController : EnemyController
{
    [Header("Boss")]
    public BossDifficulty difficulty = BossDifficulty.SmallBoss;
    public BossLootDrop lootDrop;
    public bool autoConfigureStats = true;
    public float specialAttackCooldown = 6f;
    public float specialAttackRange = 8f;
    public float specialAttackDamageMultiplier = 1.6f;
    public ParticleSystem specialAttackEffect;

    private float nextSpecialAttackTime;

    protected override void Awake()
    {
        base.Awake();

        if (lootDrop == null)
        {
            lootDrop = GetComponent<BossLootDrop>();
        }
    }

    protected override void Start()
    {
        if (autoConfigureStats)
        {
            ApplyDifficultyStats();
        }

        base.Start();

        if (lootDrop != null)
        {
            lootDrop.difficulty = difficulty;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || target == null || difficulty == BossDifficulty.SmallBoss) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= specialAttackRange && Time.time >= nextSpecialAttackTime)
        {
            UseSpecialAttack();
        }
    }

    protected override void HandleDeath()
    {
        if (lootDrop != null)
        {
            lootDrop.DropLoot();
        }

        base.HandleDeath();
    }

    private void ApplyDifficultyStats()
    {
        switch (difficulty)
        {
            case BossDifficulty.MediumBoss:
                maxHealth = 260f;
                damage = 24f;
                defense = 18f;
                movementSpeed = 3.2f;
                attackCooldown = 1.35f;
                detectionRange = 18f;
                attackRange = 2.4f;
                break;

            case BossDifficulty.HighBoss:
                maxHealth = 520f;
                damage = 38f;
                defense = 35f;
                movementSpeed = 3.6f;
                attackCooldown = 1.15f;
                detectionRange = 24f;
                attackRange = 2.8f;
                break;

            default:
                maxHealth = 160f;
                damage = 16f;
                defense = 8f;
                movementSpeed = 3f;
                attackCooldown = 1.5f;
                detectionRange = 14f;
                attackRange = 2.1f;
                break;
        }
    }

    private void UseSpecialAttack()
    {
        nextSpecialAttackTime = Time.time + specialAttackCooldown;
        SetTrigger("CastSpell");

        if (specialAttackEffect != null)
        {
            CombatVFXManager.Spawn(specialAttackEffect, transform.position + transform.forward + Vector3.up, transform.forward);
        }

        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        Vector3 hitDirection = target.position - transform.position;
        DamageData damageData = new DamageData(damage * specialAttackDamageMultiplier, DamageType.Arcane, gameObject, knockbackForce * 1.5f, false)
            .WithHit(target.position + Vector3.up, hitDirection);

        damageable.TakeDamage(damageData);
    }
}
