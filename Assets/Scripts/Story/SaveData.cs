using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public List<string> acceptedMissions = new List<string>();
    public List<string> completedMissions = new List<string>();
    public List<string> collectedItems = new List<string>();
    public int xp;
    public int finalQuestProgressCount;
    public bool finalQuestUnlocked;
    public bool finalQuestAccepted;
    public bool finalQuestComplete;
}
