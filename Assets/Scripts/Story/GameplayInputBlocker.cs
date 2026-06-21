using System.Collections.Generic;
using UnityEngine;

public static class GameplayInputBlocker
{
    private static int depth;
    private static readonly Dictionary<Behaviour, bool> previousStates = new Dictionary<Behaviour, bool>();
    private static CursorLockMode previousLockMode;
    private static bool previousCursorVisible;

    public static void Push()
    {
        depth++;
        if (depth > 1) return;

        previousStates.Clear();
        previousLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            PlayerMovement movement = Object.FindObjectOfType<PlayerMovement>();
            if (movement != null)
            {
                player = movement.gameObject;
            }
        }

        if (player != null)
        {
            Block(player.GetComponent<PlayerMovement>());
            Block(player.GetComponent<PlayerCombat>());
            Block(player.GetComponent<PlayerSpellCaster>());

            Rigidbody body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void Pop()
    {
        if (depth <= 0) return;

        depth--;
        if (depth > 0) return;

        foreach (KeyValuePair<Behaviour, bool> entry in previousStates)
        {
            if (entry.Key != null)
            {
                entry.Key.enabled = entry.Value;
            }
        }

        previousStates.Clear();
        Cursor.lockState = previousLockMode;
        Cursor.visible = previousCursorVisible;
    }

    private static void Block(Behaviour behaviour)
    {
        if (behaviour == null || previousStates.ContainsKey(behaviour)) return;

        previousStates.Add(behaviour, behaviour.enabled);
        behaviour.enabled = false;
    }
}
