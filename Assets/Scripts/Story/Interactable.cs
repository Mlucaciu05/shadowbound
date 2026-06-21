using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string Prompt { get; }
    public abstract void Interact(GameObject interactor);

    public virtual void Interact()
    {
        Interact(null);
    }
}
