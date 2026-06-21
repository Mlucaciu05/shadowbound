using NUnit.Framework;
using UnityEngine;

public class GameProgressTests
{
    private GameObject host;
    private GameProgress progress;

    [SetUp]
    public void SetUp()
    {
        SaveSystem.Delete();
        host = new GameObject("GameProgress (test)");
        progress = host.AddComponent<GameProgress>();
        progress.questsRequiredForFinalQuest = 2;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(host);
        SaveSystem.Delete();
    }

    [Test]
    public void AddXp_IncreasesXpAndLevel()
    {
        progress.AddXp(GameProgress.XpPerLevel);

        Assert.AreEqual(GameProgress.XpPerLevel, progress.Xp);
        Assert.AreEqual(2, progress.Level);
    }

    [Test]
    public void AddXp_NonPositiveAmount_IsIgnored()
    {
        progress.AddXp(0);
        progress.AddXp(-50);

        Assert.AreEqual(0, progress.Xp);
    }

    [Test]
    public void AcceptMission_ThenIsMissionAccepted_ReturnsTrue()
    {
        StoryQuestDefinition quest = new StoryQuestDefinition { missionId = "quest_a" };

        bool accepted = progress.AcceptMission(quest);

        Assert.IsTrue(accepted);
        Assert.IsTrue(progress.IsMissionAccepted("quest_a"));
    }

    [Test]
    public void AcceptMission_Twice_ReturnsFalseSecondTime()
    {
        StoryQuestDefinition quest = new StoryQuestDefinition { missionId = "quest_a" };

        progress.AcceptMission(quest);
        bool acceptedAgain = progress.AcceptMission(quest);

        Assert.IsFalse(acceptedAgain);
    }

    [Test]
    public void CompleteMission_MarksCompleteAndGrantsXp()
    {
        bool completed = progress.CompleteMission("quest_a", null, null, 100, false);

        Assert.IsTrue(completed);
        Assert.IsTrue(progress.IsMissionComplete("quest_a"));
        Assert.AreEqual(100, progress.Xp);
    }

    [Test]
    public void CompleteMission_Twice_ReturnsFalseAndDoesNotDoubleGrantXp()
    {
        progress.CompleteMission("quest_a", null, null, 100, false);
        bool completedAgain = progress.CompleteMission("quest_a", null, null, 100, false);

        Assert.IsFalse(completedAgain);
        Assert.AreEqual(100, progress.Xp);
    }

    [Test]
    public void CompleteMission_WithRewardItemId_AddsToCollectedItems()
    {
        progress.CompleteMission("quest_a", null, "sword_of_dawn", 0, false);

        Assert.IsTrue(progress.HasItem("sword_of_dawn"));
    }

    [Test]
    public void CompleteMission_CountingTowardFinalQuest_UnlocksAtThreshold()
    {
        progress.CompleteMission("quest_a", null, null, 0, true);
        Assert.IsFalse(progress.FinalQuestUnlocked);

        progress.CompleteMission("quest_b", null, null, 0, true);
        Assert.IsTrue(progress.FinalQuestUnlocked);
    }

    [Test]
    public void CompletionPercent_ScalesWithFinalQuestProgress()
    {
        progress.CompleteMission("quest_a", null, null, 0, true);

        Assert.AreEqual(50, progress.CompletionPercent());
    }

    [Test]
    public void NewGame_ResetsProgressAndDeletesSaveFile()
    {
        progress.AddXp(500);
        progress.CompleteMission("quest_a", null, null, 0, false);

        progress.NewGame();

        Assert.AreEqual(0, progress.Xp);
        Assert.IsFalse(progress.IsMissionComplete("quest_a"));
        Assert.IsNull(SaveSystem.Load());
    }

    [Test]
    public void Save_PersistsAcrossLoadFromDisk()
    {
        progress.AddXp(300);
        progress.CompleteMission("quest_a", null, null, 0, false);

        progress.LoadFromDisk(false);

        Assert.AreEqual(300, progress.Xp);
        Assert.IsTrue(progress.IsMissionComplete("quest_a"));
    }
}
