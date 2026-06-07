using System.Collections;
using System.Collections.Generic;
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

        string[] allTriggers = { "idle", "walk", "run", "walk-back", "run-back", "light-attack", "heavy-attack", "block-idle", "block-attack" };

        foreach (string trigger in allTriggers)
        {
            if (trigger == activeTrigger)
                animator.SetTrigger(trigger);
            else
                animator.ResetTrigger(trigger);
        }
    }
}