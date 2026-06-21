using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    public string gameplaySceneName = "SampleScene";

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject exitConfirmPanel;

    [Header("Buttons")]
    public Button continueButton;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.Load() != null;
        }

        SpectatorCamera cameraScript = FindObjectOfType<SpectatorCamera>();

        if (cameraScript != null)
        {
            cameraScript.enabled = false;
        }
        ShowMainPanel();
    }

    public void OnNewGameClicked()
    {
        SaveSystem.Delete();
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.NewGame();
        }

        SpectatorCamera cameraScript = FindObjectOfType<SpectatorCamera>();

        if (cameraScript != null)
        {
            cameraScript.enabled = true;
        }
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnContinueClicked()
    {
        if (SaveSystem.Load() == null) return;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.LoadFromDisk();
        }

        SpectatorCamera cameraScript = FindObjectOfType<SpectatorCamera>();

        if (cameraScript != null)
        {
            cameraScript.enabled = true;
        }
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnSettingsClicked()
    {
        SetActive(mainPanel, false);
        SetActive(settingsPanel, true);
    }

    public void OnSettingsBackClicked()
    {
        ShowMainPanel();
    }

    public void OnExitClicked()
    {
        SetActive(mainPanel, false);
        SetActive(exitConfirmPanel, true);
    }

    public void OnExitSaveAndQuitClicked()
    {
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.Save();
        }

        QuitGame();
    }

    public void OnExitWithoutSavingClicked()
    {
        QuitGame();
    }

    public void OnExitCancelClicked()
    {
        SetActive(exitConfirmPanel, false);
        SetActive(mainPanel, true);
    }

    private void ShowMainPanel()
    {
        SetActive(mainPanel, true);
        SetActive(settingsPanel, false);
        SetActive(exitConfirmPanel, false);
    }

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null)
        {
            go.SetActive(active);
        }
    }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
