using System;
using UnityEngine;

[Serializable]
public class DialogueExchange
{
    [TextArea(3, 8)] public string npcLine;
    public DialogueChoice[] choices = new DialogueChoice[3];

    public void EnsureThreeChoices()
    {
        if (choices == null || choices.Length != 3)
        {
            DialogueChoice[] newChoices = new DialogueChoice[3];
            for (int i = 0; i < newChoices.Length; i++)
            {
                if (choices != null && i < choices.Length)
                {
                    newChoices[i] = choices[i];
                }
                else
                {
                    newChoices[i] = new DialogueChoice();
                }
            }

            choices = newChoices;
        }

        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i] == null)
            {
                choices[i] = new DialogueChoice();
            }
        }
    }
}
