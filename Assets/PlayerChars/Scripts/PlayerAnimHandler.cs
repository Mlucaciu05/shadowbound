using UnityEngine;

public class PlayerAnimHandler : MonoBehaviour
{
    public Animator animator;

    void FixedUpdate()
    {
        if (animator == null) return;

        // Force the visual mesh to stay perfectly glued to the center of the capsule
        animator.transform.localPosition = new Vector3(0, -1f, 0);
        animator.transform.localRotation = Quaternion.identity;
    }

    public void SetAnimationState(string activeTrigger)
    {
        if (animator == null) return;

        string[] allTriggers = { "idle", "walk", "run", "walk-back", "run-back", "light-attack", "heavy-attack", "block-idle", "block-attack", "spell-cast", "impact", "death" };

        foreach (string trigger in allTriggers)
        {
            if (!HasParameter(trigger, AnimatorControllerParameterType.Trigger)) continue;

            if (trigger == activeTrigger)
                animator.SetTrigger(trigger);
            else
                animator.ResetTrigger(trigger);
        }

        ApplyModernAnimatorParameters(activeTrigger);
    }

    private void ApplyModernAnimatorParameters(string activeTrigger)
    {
        bool isRunning = activeTrigger == "run" || activeTrigger == "run-back";
        bool isMoving = activeTrigger == "walk" || activeTrigger == "walk-back" || isRunning;
        bool isBlocking = activeTrigger == "block-idle" || activeTrigger == "block-attack";

        SetFloat("Speed", isMoving ? (isRunning ? 1f : 0.5f) : 0f);
        SetBool("IsRunning", isRunning);
        SetBool("Block", isBlocking);

        if (activeTrigger == "light-attack" || activeTrigger == "heavy-attack" || activeTrigger == "block-attack")
        {
            SetTrigger("Attack");
        }
        else if (activeTrigger == "spell-cast")
        {
            SetTrigger("CastSpell");
        }
        else if (activeTrigger == "impact")
        {
            SetTrigger("Hit");
        }
        else if (activeTrigger == "death")
        {
            SetTrigger("Die");
        }
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
