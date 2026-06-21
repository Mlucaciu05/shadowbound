using System;
using UnityEngine;

[Serializable]
public struct DamageData
{
    public float damageAmount;
    public DamageType damageType;
    public GameObject attacker;
    public float knockbackForce;
    public bool isCritical;
    public Vector3 hitPoint;
    public Vector3 hitDirection;

    public DamageData(float damageAmount, DamageType damageType, GameObject attacker, float knockbackForce, bool isCritical)
    {
        this.damageAmount = damageAmount;
        this.damageType = damageType;
        this.attacker = attacker;
        this.knockbackForce = knockbackForce;
        this.isCritical = isCritical;
        hitPoint = Vector3.zero;
        hitDirection = Vector3.zero;
    }

    public DamageData WithHit(Vector3 point, Vector3 direction)
    {
        hitPoint = point;
        hitDirection = direction;
        return this;
    }
}
