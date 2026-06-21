using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }
    public const int XpPerLevel = 250;

    public int questsRequiredForFinalQuest = 4;
    public UnityEvent onProgressChanged = new UnityEvent();
    public UnityEvent onFinalQuestUnlocked = new UnityEvent();

    private SaveData data = new SaveData();
    private bool pendingRestorePlayerState;

    public int Xp { get { return data.xp; } }
    public int Level { get { return data.xp / XpPerLevel + 1; } }
    public int CollectedCount { get { return data.collectedItems.Count; } }
    public int CompletedMissionCount { get { return data.completedMissions.Count; } }
    public int FinalQuestProgressCount { get { return data.finalQuestProgressCount; } }
    public bool FinalQuestUnlocked { get { return data.finalQuestUnlocked; } }
    public bool FinalQuestAccepted { get { return data.finalQuestAccepted; } }
    public bool FinalQuestComplete { get { return data.finalQuestComplete; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null)
        {
            new GameObject("GameProgress (auto)").AddComponent<GameProgress>();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        LoadFromDisk(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    void OnApplicationQuit()
    {
        CapturePlayerState();
        Save();
    }

    public void NewGame()
    {
        data = new SaveData();
        SaveSystem.Delete();
        pendingRestorePlayerState = false;
        onProgressChanged?.Invoke();
    }

    public void LoadFromDisk()
    {
        LoadFromDisk(true);
    }

    public void LoadFromDisk(bool restorePlayerState)
    {
        data = SaveSystem.Load() ?? new SaveData();
        pendingRestorePlayerState = restorePlayerState && data.hasPlayerState;
        RefreshFinalQuestUnlock(false);
    }

    public void Save()
    {
        CapturePlayerState();
        SaveSystem.Save(data);
    }

    private void CapturePlayerState()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        data.hasPlayerState = true;
        data.sceneName = SceneManager.GetActiveScene().name;
        data.playerPosition = player.transform.position;
        data.playerRotationY = player.transform.eulerAngles.y;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!pendingRestorePlayerState || !data.hasPlayerState) return;
        if (!string.IsNullOrEmpty(data.sceneName) && data.sceneName != scene.name) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;
            player.transform.rotation = Quaternion.Euler(0f, data.playerRotationY, 0f);
        }

        pendingRestorePlayerState = false;
    }

    public bool IsMissionAccepted(string missionId)
    {
        return !string.IsNullOrEmpty(missionId) && data.acceptedMissions.Contains(missionId);
    }

    public bool IsMissionComplete(string missionId)
    {
        return !string.IsNullOrEmpty(missionId) && data.completedMissions.Contains(missionId);
    }

    public bool AcceptMission(StoryQuestDefinition quest)
    {
        if (quest == null || string.IsNullOrEmpty(quest.missionId)) return false;
        if (IsMissionComplete(quest.missionId) || IsMissionAccepted(quest.missionId)) return false;

        data.acceptedMissions.Add(quest.missionId);
        Save();
        onProgressChanged?.Invoke();
        return true;
    }

    public bool CompleteMission(StoryQuestDefinition quest, PlayerInventory inventory = null)
    {
        if (quest == null) return false;
        return CompleteMission(quest.missionId, quest.CreateReward(), quest.rewardItemId, quest.xpReward, quest.countsTowardFinalQuest, inventory);
    }

    public bool CompleteMission(string missionId, ItemInstance reward, string rewardItemId, int xpReward, bool countsTowardFinalQuest, PlayerInventory inventory = null)
    {
        if (string.IsNullOrEmpty(missionId)) return false;
        if (data.completedMissions.Contains(missionId)) return false;

        data.completedMissions.Add(missionId);
        if (!data.acceptedMissions.Contains(missionId))
        {
            data.acceptedMissions.Add(missionId);
        }

        if (!string.IsNullOrEmpty(rewardItemId) && !data.collectedItems.Contains(rewardItemId))
        {
            data.collectedItems.Add(rewardItemId);
        }

        if (countsTowardFinalQuest)
        {
            data.finalQuestProgressCount++;
        }

        if (reward != null)
        {
            if (inventory == null)
            {
                inventory = FindObjectOfType<PlayerInventory>();
            }

            if (inventory != null)
            {
                inventory.AddItem(reward);
            }

            LootNotificationUI.ShowMessage("You received: " + reward.DisplayName);
        }

        AddXp(xpReward, false);
        RefreshFinalQuestUnlock(countsTowardFinalQuest);
        Save();
        onProgressChanged?.Invoke();
        return true;
    }

    public bool HasItem(string itemId)
    {
        return !string.IsNullOrEmpty(itemId) && data.collectedItems.Contains(itemId);
    }

    public int AddXp(int amount)
    {
        return AddXp(amount, true);
    }

    public int AddXp(int amount, bool saveAfter)
    {
        if (amount <= 0) return data.xp;

        data.xp += amount;
        if (saveAfter)
        {
            Save();
            onProgressChanged?.Invoke();
        }

        return data.xp;
    }

    public void AcceptFinalQuest()
    {
        if (!data.finalQuestUnlocked || data.finalQuestComplete) return;

        data.finalQuestAccepted = true;
        Save();
        onProgressChanged?.Invoke();
    }

    public void CompleteFinalQuest(int xpReward = 1000)
    {
        if (data.finalQuestComplete) return;

        data.finalQuestAccepted = true;
        data.finalQuestUnlocked = true;
        data.finalQuestComplete = true;
        AddXp(xpReward, false);
        Save();
        onProgressChanged?.Invoke();
    }

    public float CompletionFraction()
    {
        if (questsRequiredForFinalQuest <= 0) return 1f;
        return Mathf.Clamp01((float)data.finalQuestProgressCount / questsRequiredForFinalQuest);
    }

    public int CompletionPercent()
    {
        return Mathf.RoundToInt(CompletionFraction() * 100f);
    }

    private void RefreshFinalQuestUnlock(bool countChange)
    {
        if (!data.finalQuestUnlocked && data.finalQuestProgressCount >= questsRequiredForFinalQuest)
        {
            data.finalQuestUnlocked = true;
            if (countChange)
            {
                onFinalQuestUnlocked?.Invoke();
                LootNotificationUI.ShowMessage("The path to the final challenge is open.");
            }
        }
    }
}
