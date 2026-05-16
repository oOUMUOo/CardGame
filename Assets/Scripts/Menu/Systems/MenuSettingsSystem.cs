using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuSettingsSystem : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SfxVolume";

    [Header("Other")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    private readonly List<Resolution> availableResolutions = new();

    private const string FullscreenKey = "Setting_Fullscreen";
    private const string ResolutionIndexKey = "Setting_ResolutionIndex";
    private const string MasterVolumeKey = "Setting_MasterVolume";
    private const string MusicVolumeKey = "Setting_MusicVolume";
    private const string SfxVolumeKey = "Setting_SfxVolume";
    private const string LanguageIndexKey = "Setting_LanguageIndex";

    private void Awake()
    {
        SetupResolutionDropdown();
        SetupLanguageDropdown();
        LoadSettings();
        BindEvents();
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            return;
        }

        resolutionDropdown.ClearOptions();
        availableResolutions.Clear();

        List<string> options = new();
        foreach (var resolution in Screen.resolutions)
        {
            if (availableResolutions.Count > 0)
            {
                Resolution prev = availableResolutions[availableResolutions.Count - 1];
                if (prev.width == resolution.width && prev.height == resolution.height)
                {
                    continue;
                }
            }

            availableResolutions.Add(resolution);
            options.Add($"{resolution.width} x {resolution.height}");
        }

        resolutionDropdown.AddOptions(options);
    }

    private void SetupLanguageDropdown()
    {
        if (languageDropdown == null)
        {
            return;
        }

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "Русский" });
    }

    private void BindEvents()
    {
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex);
        }

        if (masterSlider != null)
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(SetLanguage);
        }
    }

    private void LoadSettings()
    {
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        int resolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, GetCurrentResolutionIndex());
        float master = PlayerPrefs.GetFloat(MasterVolumeKey, 0.8f);
        float music = PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f);
        float sfx = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);
        int language = PlayerPrefs.GetInt(LanguageIndexKey, 0);

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
        }

        if (resolutionDropdown != null && resolutionDropdown.options.Count > 0)
        {
            resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolutionDropdown.options.Count - 1);
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
        }

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(master);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(music);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfx);
        if (languageDropdown != null) languageDropdown.SetValueWithoutNotify(language);

        ApplyResolution(resolutionIndex, fullscreen);
        ApplyMixerVolume(masterVolumeParam, master);
        ApplyMixerVolume(musicVolumeParam, music);
        ApplyMixerVolume(sfxVolumeParam, sfx);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetResolutionByIndex(int index)
    {
        bool fullscreen = fullscreenToggle == null || fullscreenToggle.isOn;
        ApplyResolution(index, fullscreen);
        PlayerPrefs.SetInt(ResolutionIndexKey, index);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float normalizedValue)
    {
        ApplyMixerVolume(masterVolumeParam, normalizedValue);
        PlayerPrefs.SetFloat(MasterVolumeKey, normalizedValue);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float normalizedValue)
    {
        ApplyMixerVolume(musicVolumeParam, normalizedValue);
        PlayerPrefs.SetFloat(MusicVolumeKey, normalizedValue);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float normalizedValue)
    {
        ApplyMixerVolume(sfxVolumeParam, normalizedValue);
        PlayerPrefs.SetFloat(SfxVolumeKey, normalizedValue);
        PlayerPrefs.Save();
    }

    public void SetLanguage(int languageIndex)
    {
        PlayerPrefs.SetInt(LanguageIndexKey, languageIndex);
        PlayerPrefs.Save();
    }

    private void ApplyResolution(int index, bool fullscreen)
    {
        if (availableResolutions.Count == 0)
        {
            return;
        }

        int clamped = Mathf.Clamp(index, 0, availableResolutions.Count - 1);
        Resolution resolution = availableResolutions[clamped];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }

    private void ApplyMixerVolume(string parameterName, float normalizedValue)
    {
        if (audioMixer == null || string.IsNullOrEmpty(parameterName))
        {
            return;
        }

        float clamped = Mathf.Clamp(normalizedValue, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    private int GetCurrentResolutionIndex()
    {
        if (availableResolutions.Count == 0)
        {
            return 0;
        }

        for (int i = 0; i < availableResolutions.Count; i++)
        {
            Resolution resolution = availableResolutions[i];
            if (resolution.width == Screen.currentResolution.width &&
                resolution.height == Screen.currentResolution.height)
            {
                return i;
            }
        }

        return availableResolutions.Count - 1;
    }
}
