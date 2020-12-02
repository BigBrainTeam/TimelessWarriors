using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance { get; private set; }
    public AudioMixer audioMixer;
    public AudioMixerGroup music;
    public AudioMixerGroup sfx;
    public AudioMixerGroup voices;
    public AudioSource musicSource;
    public AudioSource SFXSource;
    public Dictionary<string, AudioClip> audioclips;
    public AudioClip mainMenuMusic;
    public AudioClip selectButtonSFX;
    public AudioClip wrongSelectButtonSFX;
    public AudioClip clickSelectButtonSFX;
    public AudioClip backSelectButtonSFX;
    public AudioClip mixerchangeSFX;
    public AudioClip deathSFX;
    public AudioClip respawnSFX;
    public AudioClip hitSFX;
    public AudioClip announcer321;
    public AudioClip announcerGame;
    public AudioClip swordSFX;
    public AudioClip daggerSFX;
    public AudioClip shotSFX;
    public AudioClip lightAttack;
    public AudioClip thunderAttack;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        audioclips = new Dictionary<string, AudioClip>();
        initClips();
    }

    private void Start()
    {
        loadAudioFromSettings();
    }

    void initClips()
    {
        audioclips.Add("mainMenuMusic", mainMenuMusic);
        audioclips.Add("selectButtonSFX", selectButtonSFX);
        audioclips.Add("wrongSelectButtonSFX", wrongSelectButtonSFX);
        audioclips.Add("clickSelectButtonSFX", clickSelectButtonSFX);
        audioclips.Add("backSelectButtonSFX", backSelectButtonSFX);
        audioclips.Add("deathSFX", deathSFX);
        audioclips.Add("respawnSFX", respawnSFX);
        audioclips.Add("mixerchangeSFX", mixerchangeSFX);
        audioclips.Add("hitSFX", hitSFX);
        //audioclips.Add("announcer321", announcer321);
        //audioclips.Add("announcerGame", announcerGame);
        audioclips.Add("swordSFX", swordSFX);
        audioclips.Add("daggerSFX", daggerSFX);
        audioclips.Add("shotSFX", shotSFX);
        audioclips.Add("lightAttack", lightAttack);
        audioclips.Add("thunderAttack", thunderAttack);
    }

    public void setSFXVolume(float sfxv)
    {
        audioMixer.SetFloat("SFXVolume", sfxv);
        Settings.Instance.audioSettings.sfxv = sfxv;
    }

    public void setMusicVolume(float musicv)
    {
        audioMixer.SetFloat("musicVolume", musicv);
        Settings.Instance.audioSettings.musicv = musicv;
    }

    public void setMasterVolume(float masterv)
    {
        audioMixer.SetFloat("masterVolume", masterv);
        Settings.Instance.audioSettings.masterv = masterv;
    }

    public void setVoicesVolume(float voicesv)
    {
        audioMixer.SetFloat("voicesVolume", voicesv);
        Settings.Instance.audioSettings.voicesv = voicesv;
    }

    public void loadAudioFromSettings()
    {
        audioMixer.SetFloat("masterVolume", Settings.Instance.audioSettings.masterv);
        audioMixer.SetFloat("musicVolume", Settings.Instance.audioSettings.musicv);
        audioMixer.SetFloat("SFXVolume", Settings.Instance.audioSettings.sfxv);
        audioMixer.SetFloat("voicesVolume", Settings.Instance.audioSettings.voicesv);
    }
}
