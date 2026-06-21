using System.Collections;
using TMPro;
using UnityEngine;

public class LootNotificationUI : MonoBehaviour
{
    public static LootNotificationUI Instance { get; private set; }

    public TMP_Text messageText;
    public CanvasGroup canvasGroup;
    public float visibleTime = 2.5f;
    public float fadeTime = 0.5f;

    private Coroutine activeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        HideInstantly();
    }

    public void Show(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        activeRoutine = StartCoroutine(ShowRoutine());
    }

    public static void ShowMessage(string message)
    {
        if (Instance != null)
        {
            Instance.Show(message);
        }
    }

    private IEnumerator ShowRoutine()
    {
        SetAlpha(1f);
        yield return new WaitForSeconds(visibleTime);

        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Lerp(1f, 0f, timer / fadeTime));
            yield return null;
        }

        HideInstantly();
    }

    private void HideInstantly()
    {
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }

        if (messageText != null)
        {
            Color color = messageText.color;
            color.a = alpha;
            messageText.color = color;
        }
    }
}
