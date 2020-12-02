using UnityEngine;
using System.Collections;
using System;

public class Settings : MonoBehaviour
{
    [SerializeField]
    FightSettings fightSettingsTemplate;
    [SerializeField]
    VideoSettings videoSettingsTemplate;
    [SerializeField]
    AudioSettings audioSettingsTemplate;
    public FightSettings fightSettings;
    public VideoSettings videoSettings;
    public AudioSettings audioSettings;
    string fightSettingsPath;
    string audioSettingsPath;
    string videoSettingsPath;
    public static Settings Instance;
    void Awake()
    {
        fightSettingsPath = System.IO.Path.Combine(Application.persistentDataPath, "fight_Settings.json");
        audioSettingsPath = System.IO.Path.Combine(Application.persistentDataPath, "audio_Settings.json");
        videoSettingsPath = System.IO.Path.Combine(Application.persistentDataPath, "video_Settings.json");
        DontDestroyOnLoad(gameObject);
        if (Instance == null) Settings.Instance = this;
        else Debug.LogWarning("A previously awakened Settings MonoBehaviour exists!", gameObject);

        if (System.IO.File.Exists(fightSettingsPath))
            LoadSettings("fight");
        else
            InitializeFromDefaultFight(fightSettingsTemplate);
        if (System.IO.File.Exists(audioSettingsPath))
        {
            LoadSettings("audio");
        }
        else
            InitializeFromDefaultAudio(audioSettingsTemplate);
        if (System.IO.File.Exists(videoSettingsPath))
            LoadSettings("video");
        else
            InitializeFromDefaultVideo(videoSettingsTemplate);

    }

    private void InitializeFromDefaultFight(FightSettings fightSettingsTemplate)
    {
        Debug.LogFormat("Default Fight settings");
        fightSettings = fightSettingsTemplate;
    }
    private void InitializeFromDefaultAudio(AudioSettings audioSettingsTemplate)
    {
        Debug.LogFormat("Default Audio settings");
        audioSettings = audioSettingsTemplate; 
    }
    private void InitializeFromDefaultVideo(VideoSettings videoSettingsTemplate)
    {
        Debug.LogFormat("Default Video settings");
        videoSettings = videoSettingsTemplate;
    }

    public void LoadSettings(string mode)
    {
        switch (mode) {
            case "fight":
                Debug.LogFormat("LoadedFight settings");
                if (!fightSettings) DestroyImmediate(fightSettings);
                fightSettings = ScriptableObject.CreateInstance<FightSettings>();
                JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(fightSettingsPath), fightSettings);
                fightSettings.hideFlags = HideFlags.HideAndDontSave;
                break;
            case "video":
                Debug.LogFormat("LoadedVideo settings");
                if (!videoSettings) DestroyImmediate(videoSettings);
                videoSettings = ScriptableObject.CreateInstance<VideoSettings>();
                JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(videoSettingsPath), videoSettings);
                videoSettings.hideFlags = HideFlags.HideAndDontSave;
                break;
            case "audio":
                Debug.LogFormat("LoadedAudio settings");
                if (!audioSettings) DestroyImmediate(audioSettings);
                audioSettings = ScriptableObject.CreateInstance<AudioSettings>();
                JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(audioSettingsPath), audioSettings);
                audioSettings.hideFlags = HideFlags.HideAndDontSave;
                break;
        }
    }

    public void SaveSettings(string mode)
    {
        switch (mode)
        {
            case "fight":
                Debug.LogFormat("Saving Fight settings to {0}", fightSettingsPath);
                System.IO.File.WriteAllText(fightSettingsPath, JsonUtility.ToJson(fightSettings, true));
                break;
            case "video":
                Debug.LogFormat("Saving Video settings to {0}", videoSettingsPath);
                System.IO.File.WriteAllText(videoSettingsPath, JsonUtility.ToJson(videoSettings, true));
                break;
            case "audio":
                Debug.LogFormat("Saving Audio settings to {0}", audioSettingsPath);
                System.IO.File.WriteAllText(audioSettingsPath, JsonUtility.ToJson(audioSettings, true));
                break;
        }
    }
}