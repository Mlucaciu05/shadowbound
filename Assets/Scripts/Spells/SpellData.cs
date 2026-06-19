using UnityEngine;

public enum SpellCastMode
{
    Projectile,
    AreaAroundCaster
}

[CreateAssetMenu(fileName = "New Spell", menuName = "Shadowbound/Spell")]
public class SpellData : ScriptableObject
{
    public string spellName = "New Spell";
    [TextArea] public string description;
    public DamageType damageType = DamageType.Arcane;
    public SpellCastMode castMode = SpellCastMode.Projectile;
    public float damage = 20f;
    public float magicPowerScaling = 1f;
    public float cooldown = 1.5f;
    public float range = 18f;
    public float radius = 3f;
    public float projectileSpeed = 16f;
    public float manaCost = 0f;
    public float knockbackForce = 2f;
    public float slowPercent = 0.35f;
    public float slowDuration = 2f;
    public GameObject projectilePrefab;
    public ParticleSystem castEffect;
    public ParticleSystem impactEffect;
    public ParticleSystem areaEffect;
}
