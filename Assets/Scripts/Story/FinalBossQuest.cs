using UnityEngine;

public class FinalBossQuest : MonoBehaviour
{
    public GenericHealth health;
    public string finalBossName = "The Shadowbound King";
    public int xpReward = 1000;
    public ItemType rewardType = ItemType.Sword;
    public ItemRarity rewardRarity = ItemRarity.Legendary;
    public int rewardLevel = 15;
    public string rewardName = "Legendary Crownfall Blade";
    [TextArea(2, 5)] public string rewardDescription = "A final royal weapon cleansed after the castle is saved.";
    public PlayerInventory playerInventory;

    private bool completed;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<GenericHealth>();
        }

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }
    }

    void OnEnable()
    {
        if (health != null)
        {
            health.onDeath.AddListener(HandleFinalBossDeath);
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.onDeath.RemoveListener(HandleFinalBossDeath);
        }
    }

    private void HandleFinalBossDeath()
    {
        if (completed) return;
        completed = true;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.CompleteFinalQuest(xpReward);
        }

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        ItemInstance reward = ItemGenerator.Generate(rewardType, rewardRarity, rewardLevel);
        reward.itemName = rewardName;
        reward.description = rewardDescription;

        if (playerInventory != null)
        {
            playerInventory.AddItem(reward);
        }

        LootNotificationUI.ShowMessage("Kingdom saved. You received: " + reward.DisplayName);
    }
}
