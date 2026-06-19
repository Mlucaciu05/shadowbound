using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shadowbound/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType = ItemType.Sword;
    public ItemRarity rarity = ItemRarity.Common;
    public Sprite icon;
    public int level = 1;
    public float damageBonus;
    public float defenseBonus;
    public float strengthBonus;
    public float magicPowerBonus;
    public float criticalChanceBonus;
    public float criticalDamageBonus;
    public float blockReductionBonus;
    public float healthBonus;

    public ItemInstance CreateInstance()
    {
        return new ItemInstance(this);
    }
}
