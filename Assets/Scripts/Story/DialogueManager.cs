using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Style")]
    public TMP_FontAsset dialogueFont;
    public Color parchmentColor = new Color(0.74f, 0.62f, 0.42f, 0.97f);
    public Color inkColor = new Color(0.13f, 0.08f, 0.04f, 1f);
    public Color goldColor = new Color(0.72f, 0.48f, 0.16f, 1f);
    public Color choiceColor = new Color(0.28f, 0.15f, 0.06f, 0.94f);
    public float typewriterSecondsPerCharacter = 0.018f;

    private GameObject root;
    private CanvasGroup canvasGroup;
    private RectTransform panelRect;
    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI backstoryText;
    private TextMeshProUGUI bodyText;
    private TextMeshProUGUI instructionText;
    private Button[] choiceButtons;
    private TextMeshProUGUI[] choiceTexts;

    private string speakerName;
    private string backstory;
    private DialogueExchange[] exchanges;
    private int exchangeIndex;
    private Action onComplete;
    private Action onCancel;
    private Coroutine typeRoutine;
    private bool isOpen;
    private bool isTyping;
    private bool waitingForContinue;
    private string currentTypingText;

    public bool IsOpen { get { return isOpen; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null)
        {
            new GameObject("DialogueManager (auto)").AddComponent<DialogueManager>();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
    }

    void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Action cancel = onCancel;
            Close();
            cancel?.Invoke();
            return;
        }

        if (isTyping && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)))
        {
            FinishTypingImmediately();
            return;
        }

        if (waitingForContinue && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)))
        {
            ContinueAfterReply();
            return;
        }

        if (!isTyping && !waitingForContinue)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(2);
        }
    }

    public void ShowConversation(string speaker, string npcBackstory, DialogueExchange[] dialogueExchanges, Action complete = null, Action cancel = null)
    {
        if (dialogueExchanges == null || dialogueExchanges.Length == 0) return;

        speakerName = speaker;
        backstory = npcBackstory;
        exchanges = dialogueExchanges;
        onComplete = complete;
        onCancel = cancel;
        exchangeIndex = 0;

        foreach (DialogueExchange exchange in exchanges)
        {
            if (exchange != null)
            {
                exchange.EnsureThreeChoices();
            }
        }

        root.SetActive(true);
        isOpen = true;
        GameplayInputBlocker.Push();
        StartCoroutine(AnimateOpen());
        ShowExchange();
    }

    public void Close()
    {
        if (!isOpen) return;

        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }

        HideChoices();
        root.SetActive(false);
        isOpen = false;
        isTyping = false;
        waitingForContinue = false;
        GameplayInputBlocker.Pop();
    }

    private void ShowExchange()
    {
        if (exchangeIndex >= exchanges.Length)
        {
            Action complete = onComplete;
            Close();
            complete?.Invoke();
            return;
        }

        DialogueExchange exchange = exchanges[exchangeIndex];
        if (exchange == null)
        {
            exchangeIndex++;
            ShowExchange();
            return;
        }

        speakerText.text = speakerName;
        backstoryText.text = backstory;
        instructionText.text = "Choose your answer with 1, 2, 3";
        HideChoices();
        waitingForContinue = false;
        StartTyping(exchange.npcLine);
    }

    private void SelectChoice(int choiceIndex)
    {
        if (exchangeIndex >= exchanges.Length) return;

        DialogueExchange exchange = exchanges[exchangeIndex];
        if (exchange == null || exchange.choices == null || choiceIndex < 0 || choiceIndex >= exchange.choices.Length) return;

        DialogueChoice choice = exchange.choices[choiceIndex];
        if (choice == null) return;

        HideChoices();
        waitingForContinue = true;

        string playerLine = string.IsNullOrWhiteSpace(choice.playerLine) ? "I understand." : choice.playerLine;
        string response = string.IsNullOrWhiteSpace(choice.npcResponse) ? "Then let the road judge your courage." : choice.npcResponse;
        string replyPage = "<color=#5b2d10>You:</color> " + playerLine + "\n\n<color=#5b2d10>" + speakerName + ":</color> " + response;

        instructionText.text = exchangeIndex >= exchanges.Length - 1
            ? "Press Enter to finish"
            : "Press Enter to continue";

        StartTyping(replyPage);
    }

    private void ContinueAfterReply()
    {
        waitingForContinue = false;
        exchangeIndex++;
        ShowExchange();
    }

    private void StartTyping(string text)
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
        }

        currentTypingText = text ?? "";
        typeRoutine = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;

        string safeText = text ?? "";
        bodyText.text = safeText;
        bodyText.maxVisibleCharacters = 0;
        bodyText.ForceMeshUpdate();

        int visibleCharacters = bodyText.textInfo.characterCount;
        for (int i = 0; i <= visibleCharacters; i++)
        {
            bodyText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typewriterSecondsPerCharacter);
        }

        bodyText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
        typeRoutine = null;

        if (!waitingForContinue)
        {
            ShowChoices();
        }
    }

    private void FinishTypingImmediately()
    {
        if (typeRoutine != null)
        {
            StopCoroutine(typeRoutine);
            typeRoutine = null;
        }

        isTyping = false;

        bodyText.text = currentTypingText;
        bodyText.maxVisibleCharacters = int.MaxValue;

        if (waitingForContinue)
        {
            return;
        }

        DialogueExchange exchange = exchanges[exchangeIndex];
        bodyText.text = exchange != null ? exchange.npcLine : "";
        ShowChoices();
    }

    private void ShowChoices()
    {
        if (exchangeIndex >= exchanges.Length) return;

        DialogueExchange exchange = exchanges[exchangeIndex];
        if (exchange == null) return;

        exchange.EnsureThreeChoices();

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            DialogueChoice choice = exchange.choices[i];
            choiceButtons[i].gameObject.SetActive(true);
            choiceTexts[i].text = (i + 1) + ". " + (choice != null && !string.IsNullOrWhiteSpace(choice.playerLine) ? choice.playerLine : "Say nothing for a moment.");
        }
    }

    private void HideChoices()
    {
        if (choiceButtons == null) return;

        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator AnimateOpen()
    {
        canvasGroup.alpha = 0f;
        panelRect.localScale = Vector3.one * 0.94f;

        float timer = 0f;
        while (timer < 0.18f)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / 0.18f);
            canvasGroup.alpha = t;
            panelRect.localScale = Vector3.one * Mathf.Lerp(0.94f, 1f, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;
    }

    private void BuildUI()
    {
        root = new GameObject("DialogueCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
        root.transform.SetParent(transform, false);

        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGroup = root.GetComponent<CanvasGroup>();

        Image dim = CreateImage(root.transform, "Dim", new Color(0f, 0f, 0f, 0.35f));
        Stretch(dim.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Image panel = CreateImage(root.transform, "ParchmentPanel", parchmentColor);
        panelRect = panel.rectTransform;
        Stretch(panelRect, new Vector2(0.08f, 0.05f), new Vector2(0.92f, 0.52f), Vector2.zero, Vector2.zero);

        Image border = CreateImage(panelRect, "InkBorder", new Color(0.23f, 0.12f, 0.04f, 1f));
        Stretch(border.rectTransform, Vector2.zero, Vector2.one, new Vector2(-5f, -5f), new Vector2(5f, 5f));
        border.transform.SetAsFirstSibling();

        Image innerGlow = CreateImage(panelRect, "WarmInnerTone", new Color(0.95f, 0.78f, 0.46f, 0.18f));
        Stretch(innerGlow.rectTransform, Vector2.zero, Vector2.one, new Vector2(16f, 16f), new Vector2(-16f, -16f));

        speakerText = CreateLabel(panelRect, "Speaker", new Vector2(0.05f, 0.82f), new Vector2(0.45f, 0.95f), 34, FontStyles.Bold, goldColor, TextAlignmentOptions.Left);
        backstoryText = CreateLabel(panelRect, "Backstory", new Vector2(0.48f, 0.82f), new Vector2(0.95f, 0.95f), 18, FontStyles.Italic, new Color(0.24f, 0.13f, 0.05f, 0.85f), TextAlignmentOptions.Right);
        bodyText = CreateLabel(panelRect, "Body", new Vector2(0.05f, 0.37f), new Vector2(0.95f, 0.80f), 30, FontStyles.Normal, inkColor, TextAlignmentOptions.TopLeft);
        bodyText.enableWordWrapping = true;
        bodyText.richText = true;
        bodyText.lineSpacing = 8f;

        instructionText = CreateLabel(panelRect, "Instruction", new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.13f), 18, FontStyles.Italic, new Color(0.23f, 0.12f, 0.04f, 0.75f), TextAlignmentOptions.Left);

        choiceButtons = new Button[3];
        choiceTexts = new TextMeshProUGUI[3];

        for (int i = 0; i < 3; i++)
        {
            float top = 0.32f - i * 0.095f;
            Button button = CreateChoiceButton(panelRect, "Choice " + (i + 1), new Vector2(0.05f, top - 0.075f), new Vector2(0.95f, top));
            int capturedIndex = i;
            button.onClick.AddListener(() => SelectChoice(capturedIndex));
            choiceButtons[i] = button;
            choiceTexts[i] = button.GetComponentInChildren<TextMeshProUGUI>();
        }

        root.SetActive(false);
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, int fontSize, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        Stretch(rect, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.enableWordWrapping = true;
        if (dialogueFont != null) text.font = dialogueFont;

        return text;
    }

    private Button CreateChoiceButton(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax)
    {
        Image background = CreateImage(parent, name, choiceColor);
        Stretch(background.rectTransform, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

        Button button = background.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = choiceColor;
        colors.highlightedColor = new Color(0.47f, 0.28f, 0.11f, 1f);
        colors.pressedColor = new Color(0.18f, 0.09f, 0.03f, 1f);
        button.colors = colors;

        TextMeshProUGUI label = CreateLabel(background.transform, "Label", new Vector2(0.03f, 0.08f), new Vector2(0.97f, 0.92f), 20, FontStyles.Normal, new Color(0.98f, 0.83f, 0.52f, 1f), TextAlignmentOptions.Left);
        label.enableAutoSizing = true;
        label.fontSizeMin = 14;
        label.fontSizeMax = 22;

        background.gameObject.SetActive(false);
        return button;
    }

    private Image CreateImage(Transform parent, string name, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        Image image = obj.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
