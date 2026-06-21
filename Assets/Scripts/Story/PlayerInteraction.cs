using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 3f;
    public LayerMask interactLayers = ~0;
    public TMP_Text promptText;
    public Camera playerCamera;

    private Interactable currentTarget;

    void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen)
        {
            SetPrompt(null);
            return;
        }

        currentTarget = FindClosestInteractable();
        SetPrompt(currentTarget);

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(gameObject);
        }
    }

    private Interactable FindClosestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayers, QueryTriggerInteraction.Collide);
        Interactable closest = null;
        float bestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Interactable interactable = hit.GetComponentInParent<Interactable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }

    private void SetPrompt(Interactable interactable)
    {
        if (promptText == null) return;

        if (interactable == null)
        {
            promptText.text = "";
            promptText.gameObject.SetActive(false);
            return;
        }

        promptText.gameObject.SetActive(true);
        promptText.text = "[E] " + interactable.Prompt;
    }
}
