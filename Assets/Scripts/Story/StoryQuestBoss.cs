using UnityEngine;

public class StoryQuestBoss : MonoBehaviour
{
    public StoryQuestDefinition quest = new StoryQuestDefinition();
    public GenericHealth health;
    public PlayerInventory playerInventory;
    public bool announceQuestCompletion = true;

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
            health.onDeath.AddListener(HandleBossDeath);
        }
    }

    void OnDisable()
    {
        if (health != null)
        {
            health.onDeath.RemoveListener(HandleBossDeath);
        }
    }

    private void HandleBossDeath()
    {
        if (completed || quest == null) return;
        completed = true;

        if (playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        bool firstCompletion = GameProgress.Instance != null && GameProgress.Instance.CompleteMission(quest, playerInventory);
        if (firstCompletion && announceQuestCompletion)
        {
            LootNotificationUI.ShowMessage("Quest complete: " + quest.questTitle);
        }
    }
}
