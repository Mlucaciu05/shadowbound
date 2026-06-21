using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    public string playerLine;
    [TextArea(2, 5)] public string npcResponse;
}
