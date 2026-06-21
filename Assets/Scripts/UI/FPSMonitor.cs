using UnityEngine;
using TMPro;

public class FPSMonitor : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI label;

    [Header("Settings")]
    public float updateInterval = 0.5f;
    public KeyCode toggleKey = KeyCode.F1;
    public bool startVisible = true;

    private float timer;
    private int frameCount;
    private float fps;

    void Start()
    {
        if (label != null)
        {
            label.gameObject.SetActive(startVisible);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && label != null)
        {
            label.gameObject.SetActive(!label.gameObject.activeSelf);
        }

        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= updateInterval)
        {
            fps = frameCount / timer;
            frameCount = 0;
            timer = 0f;

            if (label != null && label.gameObject.activeInHierarchy)
            {
                label.text = string.Format("{0:0.0} FPS", fps);
            }
        }
    }

    public float CurrentFps
    {
        get { return fps; }
    }
}
