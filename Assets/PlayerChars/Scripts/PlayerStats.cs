using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Start is called before the first frame update
    public GenericHealth health;
    public PlayerCombat combat;
    public PlayerAnimHandler animHandler;

    public float blockDmgMitigation = 0.8f;

    public float damageDealt = 20f;
    public float damageMultiplier = 1f;
    void Start()
    {
        health = GetComponent<GenericHealth>();
        combat = GetComponent<PlayerCombat>();
        animHandler = GetComponent<PlayerAnimHandler>();

        health.onDeath.AddListener(HandlePlayerDeath);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ProcessIncomingDmg(float damage)
    {
        if (combat.isBlocking)
        {
            float blockedDmg = damage * blockDmgMitigation;
            damage -= blockedDmg;
            
        }

        animHandler.SetAnimationState("impact");
        health.TakeDamage(damage);
    }

    private void HandlePlayerDeath()
    {
        animHandler.SetAnimationState("death");

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
