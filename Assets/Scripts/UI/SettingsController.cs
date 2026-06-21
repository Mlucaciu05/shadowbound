using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private const string VolumeKey = "settings_volume";
    private const string FullscreenKey = "settings_fullscreen";
    private const string QualityKey = "settings_quality";

    [Header("UI References")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;

    void Start()
    {
        float volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        int quality = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());

        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.value = quality;
        }

        ApplyVolume(volume);
        ApplyFullscreen(fullscreen);
        ApplyQuality(quality);
    }

    public void OnVolumeChanged(float volume)
    {
        ApplyVolume(volume);
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    public void OnFullscreenChanged(bool fullscreen)
    {
        ApplyFullscreen(fullscreen);
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
    }

    public void OnQualityChanged(int qualityIndex)
    {
        ApplyQuality(qualityIndex);
        PlayerPrefs.SetInt(QualityKey, qualityIndex);
    }

    private static void ApplyVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    private static void ApplyFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    private static void ApplyQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }
}
