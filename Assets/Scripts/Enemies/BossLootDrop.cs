using UnityEngine;

public class BossLootDrop : MonoBehaviour
{
    public BossDifficulty difficulty = BossDifficulty.SmallBoss;
    public PlayerInventory playerInventory;
    public LootNotificationUI notificationUI;
    public ItemGenerator itemGenerator;
    public bool dropOnlyOnce = true;

    private bool hasDropped;

    void Awake()
    {
        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        if (notificationUI == null)
        {
            notificationUI = FindObjectOfType<LootNotificationUI>();
        }

        if (itemGenerator == null)
        {
            itemGenerator = FindObjectOfType<ItemGenerator>();
        }
    }

    public ItemInstance DropLoot()
    {
        if (dropOnlyOnce && hasDropped) return null;
        hasDropped = true;

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        ItemType rewardType = Random.value > 0.5f ? ItemType.Sword : ItemType.Shield;
        ItemRarity rarity = ItemGenerator.RollRarityForDifficulty(difficulty);
        int level = ItemGenerator.LevelForDifficulty(difficulty);
        ItemInstance reward = itemGenerator != null
            ? itemGenerator.GenerateItem(rewardType, rarity, level)
            : ItemGenerator.Generate(rewardType, rarity, level);

        if (playerInventory != null)
        {
            playerInventory.AddItem(reward);
        }

        string message = "You received: " + reward.DisplayName;
        if (notificationUI != null)
        {
            notificationUI.Show(message);
        }
        else
        {
            LootNotificationUI.ShowMessage(message);
        }

        return reward;
    }
}
