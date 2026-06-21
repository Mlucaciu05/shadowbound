using System.IO;
using NUnit.Framework;
using UnityEngine;

public class SaveSystemTests
{
    private const string FileName = "shadowbound_story_progress.json";

    private static string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, FileName); }
    }

    private static string BackupPath
    {
        get { return SavePath + ".bak"; }
    }

    [SetUp]
    public void SetUp()
    {
        SaveSystem.Delete();
    }

    [TearDown]
    public void TearDown()
    {
        SaveSystem.Delete();
    }

    [Test]
    public void Load_WithNoSaveFile_ReturnsNull()
    {
        Assert.IsNull(SaveSystem.Load());
    }

    [Test]
    public void SaveThenLoad_RoundTripsData()
    {
        SaveData data = new SaveData
        {
            xp = 750,
            finalQuestUnlocked = true,
            finalQuestProgressCount = 3
        };
        data.completedMissions.Add("story_quest_1");
        data.collectedItems.Add("sword_of_dawn");

        SaveSystem.Save(data);
        SaveData loaded = SaveSystem.Load();

        Assert.IsNotNull(loaded);
        Assert.AreEqual(750, loaded.xp);
        Assert.IsTrue(loaded.finalQuestUnlocked);
        Assert.AreEqual(3, loaded.finalQuestProgressCount);
        Assert.Contains("story_quest_1", loaded.completedMissions);
        Assert.Contains("sword_of_dawn", loaded.collectedItems);
    }

    [Test]
    public void Delete_RemovesSaveFile()
    {
        SaveSystem.Save(new SaveData { xp = 100 });
        SaveSystem.Delete();

        Assert.IsNull(SaveSystem.Load());
    }

    [Test]
    public void SecondSave_CreatesBackupOfPreviousSave()
    {
        SaveSystem.Save(new SaveData { xp = 100 });
        SaveSystem.Save(new SaveData { xp = 200 });

        Assert.IsTrue(File.Exists(BackupPath));
        SaveData backup = JsonUtility.FromJson<SaveData>(File.ReadAllText(BackupPath));
        Assert.AreEqual(100, backup.xp);
    }

    [Test]
    public void Load_WithCorruptMainFile_FallsBackToBackup()
    {
        SaveSystem.Save(new SaveData { xp = 100 });
        SaveSystem.Save(new SaveData { xp = 200 });

        File.WriteAllText(SavePath, "{ this is not valid json");

        SaveData loaded = SaveSystem.Load();

        Assert.IsNotNull(loaded);
        Assert.AreEqual(100, loaded.xp);
    }

    [Test]
    public void Delete_AlsoRemovesBackupFile()
    {
        SaveSystem.Save(new SaveData { xp = 100 });
        SaveSystem.Save(new SaveData { xp = 200 });

        SaveSystem.Delete();

        Assert.IsFalse(File.Exists(BackupPath));
    }
}
