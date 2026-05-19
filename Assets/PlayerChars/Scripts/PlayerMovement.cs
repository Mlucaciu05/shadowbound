using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 15f;
    public float walkBackSpeed = 3f;
    public float runBackSpeed = 7f;
    public float rotSpeed = 150f;

    [Header("Attack Dash Settings")]
    public float heavyAttackLungeSpeed = 12f; // How fast the capsule dashes forward

    private Vector3 moveDir;
    private float rotationY;
    public bool isAttacking = false;
    private float attackTimer = 0f;
    private string currentAttackType = "";

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lock rotation so physics objects don't tip the capsule over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        // 1. Handle Rotation
        if (Input.GetKey(KeyCode.A)) rotationY -= rotSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) rotationY += rotSpeed * Time.deltaTime;

        // 2. Process Animations and Look for Clicks
        HandleAnimations();

        // 3. Tick down the attack timer
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                attackTimer = 0f;
                currentAttackType = "";
            }
        }
    }

    void FixedUpdate()
    {
        // Apply character turning
        Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);
        rb.MoveRotation(targetRotation);

        // Force the visual mesh to stay perfectly glued to the center of the capsule
        animator.transform.localPosition = new Vector3(0,-1f,0);
        animator.transform.localRotation = Quaternion.identity;

        // 4. Attack Physics Handling
        if (isAttacking)
        {
            if (currentAttackType == "heavy")
            {
                // The heavy attack timer starts at 2.10 seconds.
                // We only want to push the capsule during the actual jump strike.
                // If the jump happens at the start, we dash while the timer is high.
                if (attackTimer > 0.8f && attackTimer < 1.8f)
                {
                    Vector3 lungeVelocity = transform.forward * heavyAttackLungeSpeed;
                    rb.velocity = new Vector3(lungeVelocity.x, rb.velocity.y, lungeVelocity.z);
                }
                else
                {
                    // Stop moving forward during the recovery/wind-down frames
                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                }
            }
            else
            {
                // Light attacks stay completely still
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
            return; // Bypass normal movement code
        }

        // 5. Normal Movement Physics
        float currentSpeed = 0f;
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir = transform.forward;
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir = -transform.forward;
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runBackSpeed : walkBackSpeed;
        }

        rb.velocity = new Vector3(moveDir.x * currentSpeed, rb.velocity.y, moveDir.z * currentSpeed);
    }

    void HandleAnimations()
    {
        // Mouse click inputs clear old momentum instantly and lock state
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            SetAnimationState("light-attack");
            attackTimer = 1.15f;
            isAttacking = true;
            currentAttackType = "light";
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !isAttacking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            SetAnimationState("heavy-attack");
            attackTimer = 2.10f;
            isAttacking = true;
            currentAttackType = "heavy";
            return;
        }

        // Normal movement transitions
        if (!isAttacking)
        {
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                SetAnimationState("idle");
                return;
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    SetAnimationState("run");
                else
                    SetAnimationState("walk");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    SetAnimationState("run-back");
                else
                    SetAnimationState("walk-back");
            }
        }
    }

    private void SetAnimationState(string activeTrigger)
    {
        string[] allTriggers = { "idle", "walk", "run", "walk-back", "run-back", "light-attack", "heavy-attack" };

        foreach (string trigger in allTriggers)
        {
            if (trigger == activeTrigger)
                animator.SetTrigger(trigger);
            else
                animator.ResetTrigger(trigger);
        }
    }
}