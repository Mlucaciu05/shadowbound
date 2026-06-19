using System;
using UnityEngine;

[Serializable]
public class StoryQuestDefinition
{
    public string missionId = "story_quest";
    public string questTitle = "A Kingdom in Need";
    [TextArea(2, 5)] public string questSummary;
    public string bossName = "Shadowbound Captain";
    public BossDifficulty bossDifficulty = BossDifficulty.SmallBoss;
    public ItemType rewardType = ItemType.Sword;
    public ItemRarity rewardRarity = ItemRarity.Rare;
    public int rewardLevel = 3;
    public string rewardItemId = "story_reward";
    public string rewardNameOverride;
    [TextArea(2, 5)] public string rewardDescription;
    public int xpReward = 250;
    public bool countsTowardFinalQuest = true;

    public ItemInstance CreateReward()
    {
        ItemInstance reward = ItemGenerator.Generate(rewardType, rewardRarity, Mathf.Max(1, rewardLevel));

        if (!string.IsNullOrWhiteSpace(rewardNameOverride))
        {
            reward.itemName = rewardNameOverride;
        }

        if (!string.IsNullOrWhiteSpace(rewardDescription))
        {
            reward.description = rewardDescription;
        }

        return reward;
    }
}
