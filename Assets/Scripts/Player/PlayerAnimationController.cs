using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    public Rigidbody playerBody;
    public PlayerCombat combat;
    public float runSpeedThreshold = 7f;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerBody == null)
        {
            playerBody = GetComponent<Rigidbody>();
        }

        if (combat == null)
        {
            combat = GetComponent<PlayerCombat>();
        }
    }

    void Update()
    {
        if (animator == null) return;

        float speed = 0f;
        if (playerBody != null)
        {
            Vector3 horizontalVelocity = playerBody.velocity;
            horizontalVelocity.y = 0f;
            speed = horizontalVelocity.magnitude;
        }

        SetFloat("Speed", speed);
        SetBool("IsRunning", speed >= runSpeedThreshold);
        SetBool("Block", combat != null && combat.isBlocking);
    }

    public void TriggerAttack()
    {
        SetTrigger("Attack");
    }

    public void TriggerSpellCast()
    {
        SetTrigger("CastSpell");
    }

    public void TriggerHit()
    {
        SetTrigger("Hit");
    }

    public void TriggerDeath()
    {
        SetTrigger("Die");
    }

    private void SetFloat(string parameterName, float value)
    {
        if (HasParameter(parameterName, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(parameterName, value);
        }
    }

    private void SetBool(string parameterName, bool value)
    {
        if (HasParameter(parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, value);
        }
    }

    private void SetTrigger(string parameterName)
    {
        if (HasParameter(parameterName, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(parameterName);
        }
    }

    private bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}
