using System;
using System.Collections.Generic;
using UnityEngine;

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

    public bool hasPlayerState;
    public string sceneName;
    public Vector3 playerPosition;
    public float playerRotationY;
}
