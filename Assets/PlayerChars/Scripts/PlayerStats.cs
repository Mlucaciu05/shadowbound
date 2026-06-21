using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("References")]
    public GenericHealth health;
    public PlayerCombat combat;
    public PlayerAnimHandler animHandler;
    public PlayerAnimationController animationController;
    public EquipmentManager equipmentManager;
    public Rigidbody playerBody;

    [Header("Base Stats")]
    public float baseStrength = 10f;
    public float baseDefense = 10f;
    public float baseMagicPower = 10f;
    [Range(0f, 1f)] public float baseCriticalChance = 0.05f;
    public float baseCriticalDamage = 1.5f;
    public float baseMeleeDamage = 20f;
    [Range(0f, 0.95f)] public float baseBlockReduction = 0.35f;

    [Header("Final Stats")]
    public float strength;
    public float defense;
    public float magicPower;
    [Range(0f, 1f)] public float criticalChance;
    public float criticalDamage;
    [Range(0f, 0.95f)] public float blockReduction;
    public float healthBonus;

    [Header("Legacy Combat Values")]
    public float damageDealt = 20f;
    public float damageMultiplier = 1f;

    private int baseMaxHealth;

    void Awake()
    {
        if (health == null) health = GetComponent<GenericHealth>();
        if (combat == null) combat = GetComponent<PlayerCombat>();
        if (animHandler == null) animHandler = GetComponent<PlayerAnimHandler>();
        if (animationController == null) animationController = GetComponent<PlayerAnimationController>();
        if (equipmentManager == null) equipmentManager = GetComponent<EquipmentManager>();
        if (playerBody == null) playerBody = GetComponent<Rigidbody>();
        if (health != null) baseMaxHealth = health.maxHealth;

        RecalculateStats();
    }

    void Start()
    {
        if (health != null)
        {
            health.onDeath.AddListener(HandlePlayerDeath);
        }

        RecalculateStats();
    }

    public void RecalculateStats()
    {
        strength = baseStrength;
        defense = baseDefense;
        magicPower = baseMagicPower;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
        blockReduction = baseBlockReduction;
        healthBonus = 0f;

        AddItemStats(equipmentManager != null ? equipmentManager.equippedSword : null);
        AddItemStats(equipmentManager != null ? equipmentManager.equippedShield : null);

        criticalChance = Mathf.Clamp01(criticalChance);
        criticalDamage = Mathf.Max(1f, criticalDamage);
        blockReduction = Mathf.Clamp(blockReduction, 0f, 0.95f);

        damageDealt = GetMeleeBaseDamage();

        if (health != null)
        {
            if (baseMaxHealth <= 0)
            {
                baseMaxHealth = health.maxHealth;
            }

            health.maxHealth = Mathf.RoundToInt(baseMaxHealth + healthBonus);
            health.currentHealth = Mathf.Clamp(health.currentHealth, 0f, health.maxHealth);
            health.defense = defense;
        }

        if (combat != null)
        {
            combat.weaponDmg = damageDealt;
        }
    }

    public float GetMeleeBaseDamage()
    {
        float weaponDamage = equipmentManager != null ? equipmentManager.GetWeaponDamageBonus() : 0f;
        return baseMeleeDamage + weaponDamage + strength;
    }

    public DamageData BuildMeleeDamage(string attackType, GameObject attacker, Vector3 hitPoint, Vector3 hitDirection, float knockbackForce)
    {
        bool isCritical;
        float damage = RollMeleeDamage(attackType, out isCritical);
        return new DamageData(damage, DamageType.Physical, attacker, knockbackForce, isCritical).WithHit(hitPoint, hitDirection);
    }

    public float RollMeleeDamage(string attackType, out bool isCritical)
    {
        float damage = GetMeleeBaseDamage();

        if (attackType == "light")
        {
            damage *= 0.85f;
        }
        else if (attackType == "heavy")
        {
            damage *= 1.35f;
        }
        else if (attackType == "block-attack")
        {
            damage *= 0.55f;
        }

        damage *= damageMultiplier;
        damage += Random.Range(-2f, 3f);

        isCritical = Random.value <= criticalChance;
        if (isCritical)
        {
            damage *= criticalDamage;
        }

        return Mathf.Max(1f, Mathf.Round(damage));
    }

    public DamageData BuildSpellDamage(SpellData spellData, GameObject attacker, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (spellData == null)
        {
            return new DamageData(0f, DamageType.Arcane, attacker, 0f, false).WithHit(hitPoint, hitDirection);
        }

        bool isCritical = Random.value <= criticalChance;
        float damage = spellData.damage + magicPower * spellData.magicPowerScaling;

        if (isCritical)
        {
            damage *= criticalDamage;
        }

        return new DamageData(Mathf.Round(damage), spellData.damageType, attacker, spellData.knockbackForce, isCritical).WithHit(hitPoint, hitDirection);
    }

    public float CalculateIncomingDamage(DamageData damageData, float incomingDamage)
    {
        float finalDamage = GenericHealth.ApplyDefenseReduction(incomingDamage, defense);

        if (combat != null && combat.IsBlockingWithShield())
        {
            finalDamage *= 1f - blockReduction;
        }

        return Mathf.Max(0f, finalDamage);
    }

    public bool HasShieldEquipped()
    {
        if (equipmentManager != null)
        {
            return equipmentManager.HasShieldEquipped();
        }

        return combat != null && combat.shieldTransform != null;
    }

    private void AddItemStats(ItemInstance item)
    {
        if (item == null) return;

        strength += item.strengthBonus;
        defense += item.defenseBonus;
        magicPower += item.magicPowerBonus;
        criticalChance += item.criticalChanceBonus;
        criticalDamage += item.criticalDamageBonus;
        blockReduction += item.blockReductionBonus;
        healthBonus += item.healthBonus;
    }

    private void HandlePlayerDeath()
    {
        if (animHandler != null) animHandler.SetAnimationState("death");
        if (animationController != null) animationController.TriggerDeath();

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        if (combat != null) combat.enabled = false;

        if (playerBody != null)
        {
            playerBody.velocity = Vector3.zero;
        }
    }
}
