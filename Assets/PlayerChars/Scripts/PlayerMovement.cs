using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public PlayerAnimHandler animHandler;
    public PlayerCombat combat;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 15f;
    public float walkBackSpeed = 3f;
    public float runBackSpeed = 7f;
    public float blockSpeed = 3f;
    public float rotSpeed = 150f;

    private float rotationY;
    private Vector3 moveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animHandler = GetComponent<PlayerAnimHandler>();
        combat = GetComponent<PlayerCombat>();

        // Lock rotation so physics objects don't tip the capsule over
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        rotationY = transform.localEulerAngles.y;
    }

    void Update()
    {
        // 1. Handle Turning (Always active, even while attacking)
        if (Input.GetKey(KeyCode.A)) rotationY -= rotSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) rotationY += rotSpeed * Time.deltaTime;

        // 2. Tell the animation system what to play based on inputs
        if (combat == null || !combat.isAttacking)
        {
            HandleMovementInputTransitions();
        }
    }

    void FixedUpdate()
    {
        // Apply character turning
        Quaternion targetRotation = Quaternion.Euler(0, rotationY, 0);
        if (rb == null) return;

        rb.MoveRotation(targetRotation);

        // If attacking, let PlayerCombat handle the physics updates completely
        if (combat != null && combat.isAttacking)
        {
            combat.ProcessAttackPhysics();
            return;
        }

        // Standard Movement Calculations
        float currentSpeed = 0f;
        moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir = transform.forward;
            if (combat != null && combat.isBlocking)
                currentSpeed = blockSpeed;
            else
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir = -transform.forward;
            if (combat != null && combat.isBlocking)
                currentSpeed = blockSpeed;
            else
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runBackSpeed : walkBackSpeed;
        }

        rb.velocity = new Vector3(moveDir.x * currentSpeed, rb.velocity.y, moveDir.z * currentSpeed);
    }

    private void HandleMovementInputTransitions()
    {
        if (animHandler == null) return;

        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            if (combat == null || !combat.isBlocking)
                animHandler.SetAnimationState("idle");
            else 
                animHandler.SetAnimationState("block-idle");
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (combat != null && combat.isBlocking)
                animHandler.SetAnimationState("walk");
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    animHandler.SetAnimationState("run");
                else
                    animHandler.SetAnimationState("walk");
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (combat != null && combat.isBlocking)
                animHandler.SetAnimationState("walk");
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    animHandler.SetAnimationState("run-back");
                else
                    animHandler.SetAnimationState("walk-back");
            }
        }
    }
}
