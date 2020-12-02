using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoManager : MonoBehaviour {
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdown;

    public Resolution[] resolutions;

    private void OnEnable()
    {
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasing(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }
    private void Start()
    {
        loadVideoFromSettings();
    }
    public void OnFullscreenToggle()
    {
        Settings.Instance.videoSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        Settings.Instance.videoSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnAntialiasing()
    {
        Settings.Instance.videoSettings.antialiasing = QualitySettings.antiAliasing = (int)Mathf.Pow(2f, antialiasingDropdown.value);
    }
    
    public void OnTextureQualityChange()
    {
        Settings.Instance.videoSettings.textureQuality = QualitySettings.masterTextureLimit = textureQualityDropdown.value;
    }
    public void loadVideoFromSettings()
    {
        Screen.fullScreen = Settings.Instance.videoSettings.fullscreen;
        Screen.SetResolution(resolutions[Settings.Instance.videoSettings.resolutionIndex].width, resolutions[Settings.Instance.videoSettings.resolutionIndex].height, Screen.fullScreen);
        QualitySettings.antiAliasing = Settings.Instance.videoSettings.antialiasing;
        QualitySettings.masterTextureLimit = Settings.Instance.videoSettings.textureQuality;

        fullscreenToggle.isOn = Settings.Instance.videoSettings.fullscreen;
        resolutionDropdown.value = Settings.Instance.videoSettings.resolutionIndex;
        antialiasingDropdown.value = Settings.Instance.videoSettings.antialiasing;
        textureQualityDropdown.value = Settings.Instance.videoSettings.textureQuality;
    }
}
