public static class StoryDialogueLibrary
{
    public static void ApplyPreset(NPCDialogue npc, StoryNpcPreset preset)
    {
        if (npc == null) return;

        switch (preset)
        {
            case StoryNpcPreset.ForestHealerElara:
                ApplyElara(npc);
                break;
            case StoryNpcPreset.ExiledMonkMalrec:
                ApplyMalrec(npc);
                break;
            case StoryNpcPreset.CourtSpySeraphine:
                ApplySeraphine(npc);
                break;
            default:
                ApplyAldric(npc);
                break;
        }
    }

    private static void ApplyAldric(NPCDialogue npc)
    {
        npc.npcId = "aldric";
        npc.speakerName = "Sir Aldric";
        npc.backstory = "Last surviving gate captain of the castle watch; he held the western bridge when the royal guard broke, and he still carries the guilt of leaving the king's banner behind.";
        npc.quest = new StoryQuestDefinition
        {
            missionId = "quest_gravewrought_captain",
            questTitle = "The Banner Beneath the Ashes",
            questSummary = "Aldric asks the hero to defeat the Gravewrought Captain and recover the courage of the broken watch.",
            bossName = "Gravewrought Captain",
            bossDifficulty = BossDifficulty.SmallBoss,
            rewardType = ItemType.Sword,
            rewardRarity = ItemRarity.Rare,
            rewardLevel = 4,
            rewardItemId = "oathkeeper_sword",
            rewardNameOverride = "Rare Oathkeeper Sword",
            rewardDescription = "A repaired castle blade sworn back into service after the Gravewrought Captain falls.",
            xpReward = 250
        };

        npc.firstMeetingDialogue = new[]
        {
            Ex("You walk like someone who has not yet learned what this valley takes from people. I was captain at the west gate when the shadows came through. We barred iron, burned oil, prayed loud, and still the dead climbed over us.",
                C("I came to help, not to stare at ruins.", "Then help with your hands, not your pity. The dead captain still marches below the bridge, wearing our colors like a joke."),
                C("What happened to the king's guard?", "We became a wall, then a pile, then a warning. I survived because a young soldier pushed me through a postern door and died in my place."),
                C("If the dead march, they can be broken.", "Good. Keep that steel in your voice. You will need it when old friends claw at your shield.")),
            Ex("The thing that leads them was once Captain Rovan. He taught half the castle how to hold a spear. Now he drags the king's banner through mud and calls the fallen by name.",
                C("You want me to put him down.", "I want you to free what is left of him. There is mercy in a clean end, even when the hand shakes."),
                C("Why not face him yourself?", "Because my courage is not gone, but my leg is. One bad step and I would join his patrol before sunset."),
                C("A banner can be replaced.", "A cloth can. A promise cannot. That banner is the last thing many soldiers saw before dying for this kingdom.")),
            Ex("If you defeat Rovan, take the sword from the chapel stones near his camp. I hid it there when the gate fell. Oathkeeper was not made for a coward's wall.",
                C("Then I will carry it forward.", "Do that, and perhaps the watch will stand again through you."),
                C("Will the sword answer to me?", "A sword answers to the arm. An oath answers to the heart. Bring both."),
                C("I do not fight for relics.", "Nor should you. Fight for the living who still sleep behind doors made of rotten wood.")),
            Ex("Do not mistake him for a simple corpse. He remembers formations. He waits for fear. If he raises his shield, circle left; Rovan always trusted his right side too much.",
                C("You still sound proud of him.", "I am. That is why this hurts. Evil is worse when it wears a familiar face."),
                C("I will watch his shield.", "Good. Let old training betray him one last time."),
                C("Fear will not stop me.", "Fear is honest. Let it speak, then move anyway.")),
            Ex("Go to the burned bridge when the fog thickens. Defeat the Gravewrought Captain, and the first road back to the castle opens. Bring me the silence after his final order.",
                C("I accept. The bridge will be cleared.", "Then the west gate has a champion again. May your blade remember the living."),
                C("I will return with proof.", "Return alive. Proof carried by a corpse is just another grief."),
                C("For the kingdom and the castle.", "For every name carved into these stones. Go."))
        };

        npc.reminderDialogue = new[]
        {
            Ex("The Gravewrought Captain still marches by the burned bridge. Each night he calls another dead soldier back into rank.",
                C("I am going now.", "Then keep your guard high and your feet honest."),
                C("Remind me of his weakness.", "Circle left when he shields. The living Rovan trusted his right side too much."),
                C("I need more time.", "Time is what the dead steal best. Do not gift them too much.")),
            Ex("When he falls, the road to the castle will feel less cursed. Not safe, no, but less ashamed.",
                C("I will end it.", "That is all an old captain can ask."),
                C("The watch deserves peace.", "Aye. And the young deserve a road not paved with our failures."),
                C("I will bring back Oathkeeper.", "Then perhaps oaths still mean something."))
        };

        npc.completedDialogue = new[]
        {
            Ex("I heard it from the stones before I saw you return. The bridge is quiet. Rovan is done.",
                C("He fought like a soldier.", "Then he died as close to himself as the curse allowed."),
                C("Oathkeeper is safe.", "No. Oathkeeper is awake. There is a difference."),
                C("The road is open.", "And with it, one breath of hope.")),
            Ex("Carry that blade toward the castle. If the kingdom is to stand again, it will stand behind people who chose duty after fear.",
                C("I will not waste the chance.", "Then go make the dead jealous of the living."),
                C("Thank you, Aldric.", "Thank me after the castle bells ring again."),
                C("Who should I help next?", "Find the wounded, the hungry, the ones everyone calls harmless. They know where darkness nests."))
        };

        npc.finalQuestUnlockedDialogue = new[]
        {
            Ex("You have gathered more than weapons. You have gathered witnesses. The final road is opening, and even these old walls seem to know it.",
                C("Then I am ready.", "Ready is a proud word. But you are prepared, and that is better."),
                C("The castle will be saved.", "Say it again when the last shadow answers you."),
                C("Stand with me in spirit.", "Every soldier I lost will march behind your heartbeat.")),
            Ex("Go now. The biggest challenge waits where the throne room forgot the sun.",
                C("For the kingdom.", "For the kingdom."),
                C("For the fallen.", "For the living, too."),
                C("For the castle.", "Bring its bells back."))
        };
    }

    private static void ApplyElara(NPCDialogue npc)
    {
        npc.npcId = "elara";
        npc.speakerName = "Elara Rootwhisper";
        npc.backstory = "A forest healer raised by the old grove circles; she once healed soldiers from both sides of border wars, but now the forest itself is sick and whispers with borrowed voices.";
        npc.quest = new StoryQuestDefinition
        {
            missionId = "quest_thornbound_warden",
            questTitle = "Roots Under Black Rain",
            questSummary = "Elara asks the hero to defeat the Thornbound Warden corrupting the sacred grove.",
            bossName = "Thornbound Warden",
            bossDifficulty = BossDifficulty.MediumBoss,
            rewardType = ItemType.Shield,
            rewardRarity = ItemRarity.Epic,
            rewardLevel = 7,
            rewardItemId = "verdant_aegis",
            rewardNameOverride = "Epic Verdant Aegis",
            rewardDescription = "A living shield grown from cleansed heartwood, warm to the touch when danger is near.",
            xpReward = 400
        };

        npc.firstMeetingDialogue = new[]
        {
            Ex("Do not step on the blue flowers. They are listening for footsteps that belong to the dead. I know that sounds mad, but madness has become practical lately.",
                C("The forest is speaking to you?", "It always did. Before, it spoke in roots and rain. Now it coughs up warnings like blood."),
                C("I will watch my step.", "Good. Care is the first proof that a warrior is not merely another blade with legs."),
                C("I have seen worse than flowers.", "Then you have survived sights. Now learn to survive meanings.")),
            Ex("I was healer to the outer villages. When the king still held court, people came here for feverfew, childbirth, grief, and secrets. Then black rain fell for three nights, and the grove chose a guardian too angry to die cleanly.",
                C("Who is the guardian?", "The Thornbound Warden. Once a protector. Now a crown of roots around a heart full of rot."),
                C("Can the grove be healed?", "Yes, but not while the Warden drinks every prayer before it reaches the soil."),
                C("Why would a protector turn?", "Because protection without wisdom becomes possession. He decided the forest should survive even if everyone inside it died.")),
            Ex("The Warden keeps a shield of heartwood under his roots. It was grown for the castle's first queen, who wanted defense without cruelty.",
                C("That shield could help me save people.", "Then it already knows your hand, even if you have not touched it."),
                C("Why give it to an outsider?", "Because the forest has no outsiders. Only guests who remember manners and guests who become compost."),
                C("I do not want to steal from the grove.", "You will not steal. You will remove a thorn and accept what grows free afterward.")),
            Ex("He will lash with vines, then wait for panic. Do not panic. Fire hurts him, but mercy hurts him more. Strike when the bark opens across his chest.",
                C("I can use fire.", "Use it as medicine, not hunger."),
                C("Mercy for a monster?", "Mercy is not softness. Sometimes it is the courage to end suffering."),
                C("I will watch for the opening.", "Then the grove may yet breathe through you.")),
            Ex("Go beyond the silver stream where the trees bend inward. Defeat the Thornbound Warden, and the old roads will stop leading travelers in circles.",
                C("I accept. The grove will be freed.", "Then take this blessing: may your wounds close faster than your doubts."),
                C("I will bring peace to the forest.", "Peace first comes as silence. Later, if we are lucky, birds."),
                C("The kingdom needs this road.", "The kingdom needs roots before walls. Go."))
        };

        npc.reminderDialogue = new[]
        {
            Ex("The Thornbound Warden still drinks from the grove. The trees bend lower each time I listen.",
                C("I know where to go.", "Past the silver stream, where the branches close like fingers."),
                C("Tell me his weakness again.", "Fire opens his bark, but patience wins the strike."),
                C("I am not ready yet.", "Then become ready quickly. Forests die quietly until they do not.")),
            Ex("If he falls, the road through the forest will remember the castle again.",
                C("I will return after the fight.", "Return with mud on your boots and breath in your chest."),
                C("The grove deserves better.", "So do you. So does every frightened child in the villages."),
                C("I will earn the shield.", "No. You will free it. Earning comes after."))
        };

        npc.completedDialogue = new[]
        {
            Ex("The roots loosened at dawn. I heard the Warden exhale through every tree at once.",
                C("He is gone.", "Then his watch is over, and ours begins."),
                C("The shield was waiting.", "Heartwood remembers noble hands. Let yours become one of them."),
                C("The forest felt lighter.", "That is grief leaving the body.")),
            Ex("Carry the Verdant Aegis when darkness crowds you. It was grown to protect life, not pride.",
                C("I will use it well.", "Then it will grow stronger with you."),
                C("Thank you, Elara.", "Thank the soil when you have time. It does most of the work."),
                C("What now?", "Find the truth buried under holy words. Not every shadow wears claws."))
        };

        npc.finalQuestUnlockedDialogue = new[]
        {
            Ex("The grove is blooming out of season. That means the final darkness has noticed you.",
                C("Let it notice.", "Brave. Dangerous. Sometimes necessary."),
                C("The kingdom is almost free.", "Almost is a bridge. Cross it carefully."),
                C("I am afraid.", "Good. Fear keeps the soul awake.")),
            Ex("When you face the last challenge, remember this: no castle is saved by stone. It is saved by the hands that refuse to let go.",
                C("I will remember.", "Then the forest walks beside you."),
                C("I will save them.", "Save what you can. Forgive what you cannot."),
                C("The grove gave me strength.", "And you gave it a future."))
        };
    }

    private static void ApplyMalrec(NPCDialogue npc)
    {
        npc.npcId = "malrec";
        npc.speakerName = "Brother Malrec";
        npc.backstory = "A monk exiled from the Sun Archive after accusing his own order of hiding a prophecy; he speaks like a sinner, reads like a scholar, and knows the curse began with a holy lie.";
        npc.quest = new StoryQuestDefinition
        {
            missionId = "quest_ashen_inquisitor",
            questTitle = "The Bell That Lied",
            questSummary = "Malrec sends the hero to defeat the Ashen Inquisitor and recover proof of the betrayal beneath the chapel.",
            bossName = "Ashen Inquisitor",
            bossDifficulty = BossDifficulty.MediumBoss,
            rewardType = ItemType.Sword,
            rewardRarity = ItemRarity.Epic,
            rewardLevel = 8,
            rewardItemId = "dawnfire_sword",
            rewardNameOverride = "Epic Dawnfire Sword",
            rewardDescription = "A chapel blade carrying a stubborn ember from the first royal sunrise.",
            xpReward = 450
        };

        npc.firstMeetingDialogue = new[]
        {
            Ex("If you came for a blessing, I can offer only an honest curse: may the truth find you before comfort does.",
                C("I came for truth.", "Then sit inside the wound. Truth rarely enters through polished doors."),
                C("That is a strange greeting.", "Exile improves manners by removing the audience."),
                C("I need help, not riddles.", "Then I will speak plainly: the chapel beneath the hill is lying, and its keeper kills anyone who reads too closely.")),
            Ex("I copied royal histories for twenty years. The night the shadows rose, I found a missing page sealed behind wax. It said the castle made a bargain, and the church rang bells to drown out the cost.",
                C("Who guards the page now?", "The Ashen Inquisitor. He burned his eyes to see only guilt."),
                C("The castle made a bargain?", "A desperate one. Desperation is how evil learns our handwriting."),
                C("Why should I trust an exile?", "You should not. Trust the scars on the people who called me liar.")),
            Ex("Under the chapel altar lies Dawnfire, a sword lit during the first coronation. It was meant to cut chains, not throats. The Inquisitor uses it as a threat against ghosts.",
                C("I can reclaim it.", "Do so, and the old vows may remember their purpose."),
                C("A sword from a chapel feels wrong.", "Only if prayer and defense have become strangers."),
                C("Will it hurt the final darkness?", "If your hand is steady. Dawn hates shadows by nature.")),
            Ex("The Inquisitor will accuse you as he fights. He will name sins you never committed and a few you did. Do not answer him. Strike the bell chains when he calls fire.",
                C("I will ignore his accusations.", "Harder than it sounds. Shame is a hook with a velvet handle."),
                C("Bell chains. Understood.", "Break the sound, break the spell."),
                C("What if he is right about me?", "Then improve after surviving. Dying guilty helps no one.")),
            Ex("Go beneath the ruined chapel. Defeat the Ashen Inquisitor, take Dawnfire, and bring the kingdom one step closer to remembering what it paid for this curse.",
                C("I accept. The chapel will be cleansed.", "Cleansed, no. Revealed. Clean comes later."),
                C("I will bring back proof.", "Proof is a blade too. Use both carefully."),
                C("The people deserve the truth.", "They deserve truth and bread. One day, perhaps both."))
        };

        npc.reminderDialogue = new[]
        {
            Ex("The Ashen Inquisitor waits below the chapel, burning confessions from people too dead to defend themselves.",
                C("I am ready.", "Then let his accusations pass through you like smoke."),
                C("What must I break?", "The bell chains when he calls fire."),
                C("I need more strength.", "Strength is useful. Clarity is rarer. Seek both.")),
            Ex("If Dawnfire returns to the living, the final shadow loses one of its oldest lies.",
                C("I will recover it.", "Then the archive may yet become a library instead of a tomb."),
                C("The lie ends soon.", "Lies end loudly. Brace yourself."),
                C("Pray for me.", "I do not know who listens anymore. But yes."))
        };

        npc.completedDialogue = new[]
        {
            Ex("The chapel bell cracked at sunset. I felt it in my teeth. You faced him, then.",
                C("The Inquisitor is dead.", "May his ashes finally stop preaching."),
                C("Dawnfire is mine now.", "Not yours. With you. Important distinction."),
                C("The page was real.", "I know. And I hate that I was right.")),
            Ex("Take Dawnfire toward the castle. When the final darkness speaks in royal voices, let that ember remind you that crowns can lie, but light does not negotiate.",
                C("I will carry the truth.", "Then carry it without becoming cruel."),
                C("Thank you, Brother.", "Do not make a saint of me. I am more useful as a warning."),
                C("Who else needs help?", "Power hides in courts and whispers. Find the woman who survived both."))
        };

        npc.finalQuestUnlockedDialogue = new[]
        {
            Ex("So. The pieces gather, and the prophecy stops pretending it was metaphor.",
                C("The final battle is close.", "Close enough that cowards will call it destiny."),
                C("Can we win?", "Wrong question. Ask what must remain true even if you lose."),
                C("I am tired of secrets.", "Then end the one sitting on the throne.")),
            Ex("When you enter the castle, listen for the false bell. If you hear it, remember: guilt is not command. It is only memory asking to be repaired.",
                C("I will remember.", "Good. Memory is a weapon when sharpened by mercy."),
                C("The kingdom will know the truth.", "May it survive the knowing."),
                C("Dawnfire will answer.", "Only if you do."))
        };
    }

    private static void ApplySeraphine(NPCDialogue npc)
    {
        npc.npcId = "seraphine";
        npc.speakerName = "Lady Seraphine";
        npc.backstory = "Former court whisperer, noble hostage, and keeper of the castle's secret routes; she traded gowns for knives after learning which lords opened the gates from within.";
        npc.quest = new StoryQuestDefinition
        {
            missionId = "quest_nightglass_duchess",
            questTitle = "A Mirror for Traitors",
            questSummary = "Seraphine asks the hero to defeat the Nightglass Duchess and break the spy network guarding the castle approach.",
            bossName = "Nightglass Duchess",
            bossDifficulty = BossDifficulty.HighBoss,
            rewardType = ItemType.Shield,
            rewardRarity = ItemRarity.Legendary,
            rewardLevel = 11,
            rewardItemId = "moonlit_bulwark",
            rewardNameOverride = "Legendary Moonlit Bulwark",
            rewardDescription = "A shield of silvered glass that reflects falsehoods a heartbeat before they strike.",
            xpReward = 650
        };

        npc.firstMeetingDialogue = new[]
        {
            Ex("You stand too openly for someone hunted by a kingdom's nightmares. That is not criticism. I miss that kind of innocence.",
                C("I am not innocent.", "No hero is. But some still look toward doors instead of exits."),
                C("Are you threatening me?", "If I were, you would have noticed after the knife, not before."),
                C("I need allies.", "Then learn the first rule of court: allies are people whose fears currently face the same direction.")),
            Ex("I served in the castle as a decorative hostage with excellent hearing. Lords drank wine beside me and mistook silence for stupidity. That is how I learned who sold keys to the dark.",
                C("Who betrayed the castle?", "Too many for one blade. But one still commands the secret road."),
                C("You survived the court.", "Survival is not victory. It is simply refusing to become furniture."),
                C("Tell me the name.", "The Nightglass Duchess. She smiles through mirrors and lets other people bleed.")),
            Ex("She keeps the Moonlit Bulwark in her gallery. It was made to protect royal children during sieges. She uses it to watch herself age gracefully while villages burn.",
                C("That shield belongs in battle.", "It belongs between danger and the innocent. Battle is merely where that need becomes loud."),
                C("Why does she matter to the final fight?", "Because her spies guard the hidden stair into the castle. Break her, and the castle stops seeing you coming."),
                C("I do not like nobles.", "Wise. Like individuals, distrust rooms.")),
            Ex("She will split herself in reflections. Strike the one whose shadow falls the wrong way. If all shadows look right, close your eyes for one breath and listen for silk.",
                C("That sounds dangerous.", "All useful advice does, eventually."),
                C("Wrong shadow. Silk. Understood.", "Good. You listen better than most dukes."),
                C("Can she be redeemed?", "Perhaps. But not by you while she is trying to carve your ribs.")),
            Ex("Go to the mirror gallery below the old aqueduct. Defeat the Nightglass Duchess, claim the Moonlit Bulwark, and the hidden stair to the castle will no longer belong to traitors.",
                C("I accept. Her mirrors will break.", "Try not to admire yourself in the shards."),
                C("I will open the hidden stair.", "Then the castle will learn fear can climb too."),
                C("The people she hurt deserve justice.", "Justice, yes. Revenge only if you can afford the interest."))
        };

        npc.reminderDialogue = new[]
        {
            Ex("The Nightglass Duchess is still in her gallery, rehearsing grief she plans to perform after everyone useful dies.",
                C("I know the way.", "Below the old aqueduct. Bring patience and something sharp."),
                C("Tell me the trick again.", "Strike the reflection whose shadow lies. If shadows fail you, listen for silk."),
                C("She sounds powerful.", "She is. That is why cowards gave her rooms instead of chains.")),
            Ex("When she falls, the hidden stair opens. That stair may be the difference between saving the castle and dying politely at its gate.",
                C("I will defeat her.", "Then let us both pretend I did not just become hopeful."),
                C("The shield will help.", "The shield will test you first."),
                C("I will return.", "Return with fewer holes than plans."))
        };

        npc.completedDialogue = new[]
        {
            Ex("The mirrors went dark. I assume that means you won, unless the Duchess has taken up subtlety in death.",
                C("She is gone.", "Then several traitors just became unemployed."),
                C("The shield is powerful.", "It was made for frightened children. That is the noblest kind of power."),
                C("The hidden stair is open.", "Excellent. Let the castle lose sleep.")),
            Ex("Carry the Moonlit Bulwark into the final challenge. When lies strike first, let them see themselves coming.",
                C("I will use it well.", "Do. Legendary things hate being wasted."),
                C("Thank you, Seraphine.", "You may thank me by surviving. I collect rare things."),
                C("The castle is close.", "Close enough to hurt. Keep going."))
        };

        npc.finalQuestUnlockedDialogue = new[]
        {
            Ex("So the hero gathers relics, wounds, and inconvenient hope. The court would have hated you.",
                C("Good.", "Very good answer."),
                C("I am ready for the castle.", "No one is ready for a castle that remembers betrayal. Go anyway."),
                C("Will the hidden stair hold?", "It will hold long enough. That is all history ever promises.")),
            Ex("When you face the last shadow, do not argue with it. Tyrants love debates. End the conversation.",
                C("I will end it.", "There. That almost sounded royal."),
                C("For everyone used by the court.", "For everyone who survived being useful."),
                C("For the kingdom.", "For the kingdom, then. Try not to die dramatically."))
        };
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
