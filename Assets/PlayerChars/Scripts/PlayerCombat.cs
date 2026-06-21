using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAnimHandler animHandler;
    private PlayerAnimationController animationController;
    private PlayerStats playerStats;
    private EquipmentManager equipmentManager;

    [Header("Attack State")]
    public bool isAttacking = false;
    public float attackTimer = 0f;
    public string currentAttackType = "";
    public bool isBlocking = false;
    public float nextAttackTime = 0f;

    [Header("Input")]
    public KeyCode heavyAttackModifier = KeyCode.LeftShift;
    public KeyCode legacyBlockToggleKey = KeyCode.Q;
    public bool allowLegacyBlockToggle = true;
    private bool legacyBlockToggle;

    [Header("Attack Timing")]
    public float lightAttackDuration = 1.15f;
    public float heavyAttackDuration = 2.10f;
    public float blockAttackDuration = 1.15f;
    public float lightAttackCooldown = 0.6f;
    public float heavyAttackCooldown = 1.2f;
    public float blockAttackCooldown = 0.75f;

    [Header("Range Detection")]
    public Transform attackOrigin;
    public float attackRange = 2.1f;
    public float attackRadius = 0.75f;
    public LayerMask attackLayers = ~0;
    public bool useOverlapAttackWhenNoHitbox = true;
    public float baseKnockbackForce = 4f;

    [Header("Weapons")]
    public Transform shieldTransform;
    private MeshRenderer shieldRenderer; // Keeps track of the shield's surface
    private Color originalColor;         // Saves your default texture tint

    [ColorUsage(true, true)] // Enables HDR color selection for a cool neon bloom glow!
    public Color blockingBlueColor = new Color(0f, 0.4f, 1f, 1f);

    public Collider swordCollider;
    public WeaponHitbox weaponHitbox;
    public float weaponDmg = 20f;
    public float dmgMultiplier = 1f;

    [Header("Shield Static Positions")]
    // Uncommented and optimized! These force the shield to sit exactly where you want
    public Vector3 normalShieldPos = new Vector3(0.001f, -0.171f, 0.415f);
    public Vector3 normalShieldRot = new Vector3(76.738f, 251.221f, 71.688f);
    public Vector3 blockingShieldPos = new Vector3(0.016f, 0.389f, 0.199f);
    public Vector3 blockingShieldRot = new Vector3(10.435f, 341.771f, 177.042f);

    [Header("Attack Dash Settings")]
    public float heavyAttackLungeSpeed = 12f;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animHandler = GetComponent<PlayerAnimHandler>();
        animationController = GetComponent<PlayerAnimationController>();
        playerStats = GetComponent<PlayerStats>();
        equipmentManager = GetComponent<EquipmentManager>();

        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }

        // Cache the renderer so we don't cause lag loops
        if (shieldTransform != null)
        {
            shieldRenderer = shieldTransform.GetComponent<MeshRenderer>();
            if (shieldRenderer != null)
            {
                // Save the original color of your shield texture
                originalColor = shieldRenderer.material.HasProperty("_BaseColor") ? shieldRenderer.material.GetColor("_BaseColor") : shieldRenderer.material.color;
            }
        }

        if (swordCollider != null)
        {
            weaponHitbox = swordCollider.GetComponent<WeaponHitbox>();
            if (weaponHitbox != null)
            {
                weaponHitbox.owner = this.gameObject;
                weaponHitbox.combat = this;
            }
        
            swordCollider.enabled = false;
        }
    }

    void Update()
    {
        HandleBlockInput();
        HandleAttackInput();

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }
    }

    private void HandleBlockInput()
    {
        if (allowLegacyBlockToggle && Input.GetKeyDown(legacyBlockToggleKey))
        {
            legacyBlockToggle = !legacyBlockToggle;
        }

        bool wantsBlock = Input.GetKey(KeyCode.Mouse1) || legacyBlockToggle;
        bool canBlock = CanBlock();

        if (!canBlock)
        {
            legacyBlockToggle = false;
        }

        SetBlocking(wantsBlock && canBlock);
    }

    private void HandleAttackInput()
    {
        if (isAttacking || Time.time < nextAttackTime) return;

        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        if (isBlocking)
        {
            StartAttack("block-attack", "block-attack", blockAttackDuration, blockAttackCooldown);
        }
        else if (Input.GetKey(heavyAttackModifier))
        {
            StartAttack("heavy-attack", "heavy", heavyAttackDuration, heavyAttackCooldown);
        }
        else
        {
            StartAttack("light-attack", "light", lightAttackDuration, lightAttackCooldown);
        }
    }

    private void StartAttack(string animTrigger, string attackType, float duration, float cooldown)
    {
        if (movement != null && movement.rb != null)
        {
            movement.rb.velocity = new Vector3(0, movement.rb.velocity.y, 0);
        }

        if (animHandler != null) animHandler.SetAnimationState(animTrigger);
        if (animationController != null) animationController.TriggerAttack();

        attackTimer = duration;
        isAttacking = true;
        currentAttackType = attackType;
        nextAttackTime = Time.time + cooldown;

        if (playerStats != null)
        {
            playerStats.damageMultiplier = dmgMultiplier;
            weaponDmg = playerStats.GetMeleeBaseDamage();
        }

        if (weaponHitbox != null)
        {
            weaponHitbox.damageDealt = weaponDmg;
            weaponHitbox.damageMultiplier = dmgMultiplier;
            weaponHitbox.ResetHitList();
        }

        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
        else if (useOverlapAttackWhenNoHitbox)
        {
            PerformOverlapAttack();
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        currentAttackType = "";

        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }

        if (isBlocking)
        {
            if (animHandler != null) animHandler.SetAnimationState("block-idle");
        }

        if (weaponHitbox != null)
        {
            weaponHitbox.damageDealt = weaponDmg;
            weaponHitbox.damageMultiplier = dmgMultiplier;
            weaponHitbox.ResetHitList();
        }
    }

    public void ProcessAttackPhysics()
    {
        if (movement == null || movement.rb == null) return;

        if (currentAttackType == "heavy")
        {
            if (attackTimer > 0.8f && attackTimer < 1.8f)
            {
                Vector3 lungeVelocity = transform.forward * heavyAttackLungeSpeed;
                movement.rb.velocity = new Vector3(lungeVelocity.x, movement.rb.velocity.y, lungeVelocity.z);
            }
            else
            {
                movement.rb.velocity = new Vector3(0, movement.rb.velocity.y, 0);
            }
        }
        else if (currentAttackType == "light" || currentAttackType == "block-attack")
        {
            movement.rb.velocity = new Vector3(0, movement.rb.velocity.y, 0);
        }
    }

    public DamageData BuildDamageData(GameObject target, Vector3 hitPoint)
    {
        Vector3 direction = target != null ? target.transform.position - transform.position : transform.forward;
        direction.y = 0f;

        float knockbackForce = currentAttackType == "heavy" ? baseKnockbackForce * 1.5f : baseKnockbackForce;

        if (playerStats != null)
        {
            return playerStats.BuildMeleeDamage(currentAttackType, gameObject, hitPoint, direction, knockbackForce);
        }

        bool isCritical = false;
        float damage = weaponDmg * dmgMultiplier;
        return new DamageData(damage, DamageType.Physical, gameObject, knockbackForce, isCritical).WithHit(hitPoint, direction);
    }

    public bool IsBlockingWithShield()
    {
        return isBlocking && CanBlock();
    }

    private bool CanBlock()
    {
        if (playerStats != null)
        {
            return playerStats.HasShieldEquipped();
        }

        if (equipmentManager != null)
        {
            return equipmentManager.HasShieldEquipped();
        }

        return shieldTransform != null;
    }

    private void SetBlocking(bool shouldBlock)
    {
        if (isBlocking == shouldBlock) return;

        isBlocking = shouldBlock;
        UpdateShieldVisual(isBlocking);

        if (!isAttacking && animHandler != null)
        {
            animHandler.SetAnimationState(isBlocking ? "block-idle" : "idle");
        }
    }

    private void UpdateShieldVisual(bool blocking)
    {
        if (shieldTransform != null)
        {
            shieldTransform.localPosition = blocking ? blockingShieldPos : normalShieldPos;
            shieldTransform.localEulerAngles = blocking ? blockingShieldRot : normalShieldRot;
        }

        if (shieldRenderer != null)
        {
            if (shieldRenderer.material.HasProperty("_BaseColor"))
            {
                shieldRenderer.material.SetColor("_BaseColor", blocking ? blockingBlueColor : originalColor);
            }
            else
            {
                shieldRenderer.material.color = blocking ? blockingBlueColor : originalColor;
            }
        }
    }

    private void PerformOverlapAttack()
    {
        Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position;
        Vector3 center = origin + transform.forward * attackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, attackRadius, attackLayers, QueryTriggerInteraction.Ignore);

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.gameObject == gameObject || hit.transform.IsChildOf(transform)) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable.IsDead) continue;

            Vector3 toTarget = damageable.DamageTransform.position - origin;
            toTarget.y = 0f;
            if (Vector3.Dot(transform.forward, toTarget.normalized) < 0.25f) continue;

            DamageData damageData = BuildDamageData(damageable.DamageTransform.gameObject, damageable.DamageTransform.position);
            damageable.TakeDamage(damageData);
        }
    }
}
