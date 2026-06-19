using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public ItemInstance GenerateItem(ItemType itemType, ItemRarity rarity, int level)
    {
        return Generate(itemType, rarity, level);
    }

    public static ItemInstance Generate(ItemType itemType, ItemRarity rarity, int level)
    {
        level = Mathf.Max(1, level);
        float rarityMultiplier = ItemInstance.GetRarityMultiplier(rarity);
        string itemName = BuildName(itemType, rarity);

        ItemInstance item = new ItemInstance
        {
            itemName = itemName,
            description = "Generated " + rarity + " " + itemType + ".",
            itemType = itemType,
            rarity = rarity,
            level = level
        };

        if (itemType == ItemType.Sword)
        {
            item.damageBonus = Mathf.Round((8f + level * 4f) * rarityMultiplier);
            item.strengthBonus = Mathf.Round((2f + level * 1.5f) * rarityMultiplier);
            item.criticalChanceBonus = 0.02f * rarityMultiplier;
            item.criticalDamageBonus = 0.1f * rarityMultiplier;
        }
        else if (itemType == ItemType.Shield)
        {
            item.defenseBonus = Mathf.Round((10f + level * 5f) * rarityMultiplier);
            item.blockReductionBonus = Mathf.Clamp01(0.08f + 0.04f * rarityMultiplier);
            item.healthBonus = Mathf.Round((10f + level * 6f) * rarityMultiplier);
        }

        if (rarity == ItemRarity.Legendary)
        {
            item.magicPowerBonus += Mathf.Round(5f + level * 2f);
        }

        return item;
    }

    public static ItemRarity RollRarityForDifficulty(BossDifficulty difficulty)
    {
        float roll = Random.value * 100f;

        switch (difficulty)
        {
            case BossDifficulty.MediumBoss:
                if (roll < 15f) return ItemRarity.Basic;
                if (roll < 50f) return ItemRarity.Common;
                if (roll < 80f) return ItemRarity.Rare;
                if (roll < 95f) return ItemRarity.Epic;
                return ItemRarity.Legendary;

            case BossDifficulty.HighBoss:
                if (roll < 15f) return ItemRarity.Common;
                if (roll < 50f) return ItemRarity.Rare;
                if (roll < 80f) return ItemRarity.Epic;
                return ItemRarity.Legendary;

            default:
                if (roll < 40f) return ItemRarity.Basic;
                if (roll < 75f) return ItemRarity.Common;
                if (roll < 95f) return ItemRarity.Rare;
                return ItemRarity.Epic;
        }
    }

    public static int LevelForDifficulty(BossDifficulty difficulty)
    {
        switch (difficulty)
        {
            case BossDifficulty.MediumBoss:
                return Random.Range(4, 8);
            case BossDifficulty.HighBoss:
                return Random.Range(8, 13);
            default:
                return Random.Range(1, 5);
        }
    }

    private static string BuildName(ItemType itemType, ItemRarity rarity)
    {
        string material;

        switch (rarity)
        {
            case ItemRarity.Common:
                material = "Steel";
                break;
            case ItemRarity.Rare:
                material = "Knight";
                break;
            case ItemRarity.Epic:
                material = "Flame";
                break;
            case ItemRarity.Legendary:
                material = "Dragon";
                break;
            default:
                material = "Iron";
                break;
        }

        return rarity + " " + material + " " + itemType;
    }
}
