using UnityEngine;

public class FinalChallengeNPC : Interactable
{
    public string speakerName = "Castellan Rowan";
    [TextArea(3, 8)] public string backstory = "Keeper of the sealed stair and last witness to the royal family's disappearance; he has waited beside the castle gate while hope returned piece by piece.";
    public string finalBossName = "The Shadowbound King";

    public DialogueExchange[] lockedDialogue;
    public DialogueExchange[] readyDialogue;
    public DialogueExchange[] completedDialogue;

    public override string Prompt
    {
        get
        {
            if (GameProgress.Instance != null && GameProgress.Instance.FinalQuestComplete)
            {
                return "Speak of the saved kingdom";
            }

            return "Ask about the final challenge";
        }
    }

    void Reset()
    {
        lockedDialogue = new[]
        {
            Ex("The castle gate is not closed by iron. It is closed by memory. The kingdom will not let another hopeful soul enter unprepared.",
                C("What do I still need?", "Help the people who still hold pieces of the kingdom's courage. Their rewards are more than weapons; they are permission to continue."),
                C("I can fight now.", "You can swing now. Fighting the final shadow asks more than arms."),
                C("The kingdom cannot wait.", "It has waited through ruin. It can wait until you are not simply brave, but ready.")),
            Ex("Return when the old roads, the grove, the chapel, and the hidden stair have answered you. Then the castle will know your name.",
                C("I will gather what is needed.", "Good. Purpose is built one promise at a time."),
                C("Who decides I am ready?", "The people you help. The dead have had their vote for too long."),
                C("I will come back stronger.", "Come back kinder too. Power alone is how we arrived here.")),
            Ex("Every relic you earn is a story choosing to stand behind you. A blade from duty, a shield from mercy, a flame from truth, a mirror against lies.",
                C("So the items are proof.", "Proof, yes, but also burden. They ask you to remember why you fight."),
                C("I thought strength was enough.", "Strength opens doors. Meaning tells you which ones should stay closed."),
                C("The people matter more than relics.", "Exactly. The relics only matter because people bled to keep their hope alive.")),
            Ex("The final shadow feeds on lonely heroes. That is why you must not arrive as one.",
                C("I will carry their stories.", "Then you will not enter alone."),
                C("I do not want to fail them.", "Good. Let that fear sharpen you without owning you."),
                C("I understand now.", "Understanding is the first key. Action is the second."))
        };

        readyDialogue = new[]
        {
            Ex("The gate heard you before I did. Four burdens lifted, four witnesses restored. The castle is afraid of you now.",
                C("Then open the way.", "Not open. Unseal. The difference is paid in blood."),
                C("I am ready.", "You are prepared. Ready is something people claim before history corrects them."),
                C("Who waits inside?", "The Shadowbound King, or what wears his grief like a crown.")),
            Ex("He was once the king, if the old songs are kind. The curse hollowed him and left something wearing royal grief like armor.",
                C("Can he be saved?", "Perhaps in death. Perhaps that is the last mercy left to him."),
                C("Then I will break the curse.", "Break the crown it hides inside, and the curse will bleed."),
                C("I will not fear a king.", "Fear him. Then fight him anyway.")),
            Ex("Inside, the castle will show you what you failed, what you lost, and what you secretly want. It lies best when it uses your own voice.",
                C("How do I resist that?", "Answer with what you did, not what you fear."),
                C("I know who I am.", "Hold that knowledge lightly. Pride is easy to forge into chains."),
                C("The NPCs trusted me.", "Then borrow their trust when yours runs thin.")),
            Ex("When the throne room darkens, do not chase every shadow. Strike the one that casts no reflection in the Moonlit Bulwark.",
                C("The shield will reveal him.", "For a heartbeat. That is usually all destiny offers."),
                C("And Dawnfire?", "Use it when he calls the old bargain by name."),
                C("What about Oathkeeper?", "Let it remind your hands that promises can survive kings.")),
            Ex("If you enter, the final challenge begins. Defeat the Shadowbound King and the castle bells may ring for the living again.",
                C("I accept the final quest.", "Then go with every oath, root, flame, and secret you earned."),
                C("For the kingdom.", "For the kingdom that still chooses morning."),
                C("For the castle.", "Bring it back to us."))
        };

        completedDialogue = new[]
        {
            Ex("The bells rang. Not loudly at first. Like they were afraid joy might be another trick.",
                C("The kingdom is saved.", "Saved enough to begin healing. That is the only honest kind."),
                C("The castle is free.", "Then its stones can finally become home again."),
                C("What happens now?", "Now people return, argue, rebuild, plant bread, and tell the story badly until it becomes legend.")),
            Ex("You came here as a wanderer and leave as the reason children will draw swords in the dirt and pretend courage is easy.",
                C("It was not easy.", "That is why it mattered."),
                C("I had help.", "Heroes always do. Only statues stand alone."),
                C("The kingdom belongs to its people.", "At last, someone worthy said it.")),
            Ex("The castle will need masons, farmers, teachers, and fools brave enough to sing near broken windows.",
                C("That sounds like peace.", "Peace always sounds smaller than war. That is why it is worth protecting."),
                C("Will they remember?", "They will remember badly, beautifully, and together."),
                C("I will help rebuild.", "Then the hero becomes something rarer: useful after victory."))
        };
    }

    public override void Interact(GameObject interactor)
    {
        if (DialogueManager.Instance == null || GameProgress.Instance == null) return;

        if (GameProgress.Instance.FinalQuestComplete)
        {
            DialogueManager.Instance.ShowConversation(speakerName, backstory, completedDialogue);
        }
        else if (GameProgress.Instance.FinalQuestUnlocked)
        {
            DialogueManager.Instance.ShowConversation(speakerName, backstory, readyDialogue, () =>
            {
                GameProgress.Instance.AcceptFinalQuest();
                LootNotificationUI.ShowMessage("Final quest accepted: defeat " + finalBossName);
            });
        }
        else
        {
            DialogueManager.Instance.ShowConversation(speakerName, backstory, lockedDialogue);
        }
    }

    private static DialogueExchange Ex(string line, DialogueChoice a, DialogueChoice b, DialogueChoice c)
    {
        return new DialogueExchange
        {
            npcLine = line,
            choices = new[] { a, b, c }
        };
    }

    private static DialogueChoice C(string playerLine, string npcResponse)
    {
        return new DialogueChoice
        {
            playerLine = playerLine,
            npcResponse = npcResponse
        };
    }
}
