using UnityEngine;

public class NPCDialogue : Interactable
{
    [Header("Identity")]
    public StoryNpcPreset preset = StoryNpcPreset.CastleGuardAldric;
    public bool usePresetOnReset = true;
    public string npcId = "npc";
    public string speakerName = "Villager";
    [TextArea(3, 8)] public string backstory;

    [Header("Quest")]
    public StoryQuestDefinition quest = new StoryQuestDefinition();
    public Transform bossLocationHint;

    [Header("Dialogue")]
    public DialogueExchange[] firstMeetingDialogue;
    public DialogueExchange[] reminderDialogue;
    public DialogueExchange[] completedDialogue;
    public DialogueExchange[] finalQuestUnlockedDialogue;

    public override string Prompt
    {
        get
        {
            if (GameProgress.Instance != null && quest != null)
            {
                if (GameProgress.Instance.IsMissionComplete(quest.missionId))
                {
                    return "Speak with " + speakerName;
                }

                if (GameProgress.Instance.IsMissionAccepted(quest.missionId))
                {
                    return "Ask about " + quest.bossName;
                }
            }

            return "Talk to " + speakerName;
        }
    }

    void Reset()
    {
        if (usePresetOnReset)
        {
            ApplySelectedPreset();
        }
    }

    [ContextMenu("Apply Selected Preset")]
    public void ApplySelectedPreset()
    {
        StoryDialogueLibrary.ApplyPreset(this, preset);
    }

    public override void Interact(GameObject interactor)
    {
        DialogueManager dialogue = DialogueManager.Instance;
        if (dialogue == null) return;

        GameProgress progress = GameProgress.Instance;
        if (progress == null)
        {
            new GameObject("GameProgress (auto)").AddComponent<GameProgress>();
            progress = GameProgress.Instance;
        }

        DialogueExchange[] dialogueToShow = firstMeetingDialogue;
        System.Action onComplete = () => AcceptQuest(progress);

        if (quest != null && progress.IsMissionComplete(quest.missionId))
        {
            dialogueToShow = progress.FinalQuestUnlocked && finalQuestUnlockedDialogue != null && finalQuestUnlockedDialogue.Length > 0
                ? finalQuestUnlockedDialogue
                : completedDialogue;
            onComplete = null;
        }
        else if (quest != null && progress.IsMissionAccepted(quest.missionId))
        {
            dialogueToShow = reminderDialogue;
            onComplete = null;
        }

        if (dialogueToShow == null || dialogueToShow.Length == 0)
        {
            dialogueToShow = firstMeetingDialogue;
        }

        dialogue.ShowConversation(speakerName, backstory, dialogueToShow, onComplete);
    }

    public void AcceptQuest(GameProgress progress)
    {
        if (progress == null || quest == null) return;

        bool accepted = progress.AcceptMission(quest);
        if (accepted)
        {
            string message = "Quest accepted: " + quest.questTitle;
            if (!string.IsNullOrEmpty(quest.bossName))
            {
                message += " - defeat " + quest.bossName;
            }

            LootNotificationUI.ShowMessage(message);
        }
    }
}
