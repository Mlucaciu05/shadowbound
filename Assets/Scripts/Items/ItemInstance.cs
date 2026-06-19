using System;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType;
    public ItemRarity rarity;
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

    public string DisplayName
    {
        get
        {
            if (!string.IsNullOrEmpty(itemName)) return itemName;
            return rarity + " " + itemType;
        }
    }

    public ItemInstance()
    {
    }

    public ItemInstance(ItemData data)
    {
        if (data == null) return;

        itemName = data.itemName;
        description = data.description;
        itemType = data.itemType;
        rarity = data.rarity;
        icon = data.icon;
        level = Mathf.Max(1, data.level);
        damageBonus = data.damageBonus;
        defenseBonus = data.defenseBonus;
        strengthBonus = data.strengthBonus;
        magicPowerBonus = data.magicPowerBonus;
        criticalChanceBonus = data.criticalChanceBonus;
        criticalDamageBonus = data.criticalDamageBonus;
        blockReductionBonus = data.blockReductionBonus;
        healthBonus = data.healthBonus;
    }

    public ItemInstance Clone()
    {
        return (ItemInstance)MemberwiseClone();
    }

    public static float GetRarityMultiplier(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return 1.15f;
            case ItemRarity.Rare:
                return 1.45f;
            case ItemRarity.Epic:
                return 1.9f;
            case ItemRarity.Legendary:
                return 2.6f;
            default:
                return 1f;
        }
    }
}
