using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAnimHandler animHandler;

    [Header("Attack State")]
    public bool isAttacking = false;
    public float attackTimer = 0f;
    public string currentAttackType = "";
    public bool isBlocking = false;

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

        // Cache the renderer so we don't cause lag loops
        if (shieldTransform != null)
        {
            shieldRenderer = shieldTransform.GetComponent<MeshRenderer>();
            if (shieldRenderer != null)
            {
                // Save the original color of your shield texture
                originalColor = shieldRenderer.material.GetColor("_BaseColor");
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBlocking = !isBlocking;
            if (isBlocking)
            {
                animHandler.SetAnimationState("block-idle");

                // 1. Force position and rotation anchors instantly (Bypasses .Rotate breakdown)
                if (shieldTransform != null)
                {
                    shieldTransform.localPosition = blockingShieldPos;
                    shieldTransform.localEulerAngles = blockingShieldRot;
                }

                // 2. Turn the shield glowing blue!
                if (shieldRenderer != null)
                {
                    shieldRenderer.material.SetColor("_BaseColor", blockingBlueColor);
                }
            }
            else
            {
                // 1. Restore normal position values
                if (shieldTransform != null)
                {
                    shieldTransform.localPosition = normalShieldPos;
                    shieldTransform.localEulerAngles = normalShieldRot;
                }

                // 2. Restore normal texture look
                if (shieldRenderer != null)
                {
                    shieldRenderer.material.SetColor("_BaseColor", originalColor);
                }
            }
        }

        // --- Rest of your input code stays exactly the same ---
        if (!isBlocking)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
            {
                StartAttack("light-attack", "light", 1.15f);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1) && !isAttacking)
            {
                StartAttack("heavy-attack", "heavy", 2.10f);
            }
        }
        else
        {
            if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) && !isAttacking)
            {
                StartAttack("block-attack", "block-attack", 1.15f);
            }
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                EndAttack();
            }
        }
    }

    private void StartAttack(string animTrigger, string attackType, float duration)
    {
        movement.rb.velocity = new Vector3(0, movement.rb.velocity.y, 0);
        animHandler.SetAnimationState(animTrigger);
        attackTimer = duration;
        isAttacking = true;
        currentAttackType = attackType;

        if (weaponHitbox != null) { 
            weaponHitbox.ProcessDamage(weaponDmg, dmgMultiplier);
            weaponHitbox.ResetHitList();
        }
        if(swordCollider!=null)
        {
            swordCollider.enabled = true;
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0f;
        currentAttackType = "";

        if(swordCollider != null) {swordCollider.enabled = false;
        }

        if (isBlocking)
        {
            animHandler.SetAnimationState("block-idle");
        }

        weaponHitbox.damageDealt = weaponDmg;
        weaponHitbox.damageMultiplier = dmgMultiplier;
        weaponHitbox.ResetHitList();
    }

    public void ProcessAttackPhysics()
    {
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
}