using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the Fight of the game. Has every prefab for Instantiations.
/// </summary>
public class GameManager : MonoBehaviour {
    /// <summary>
    /// Current Fight being played.
    /// </summary>
    public Fight currentFight;
    public Dictionary<string, GameObject> mapPrefabs;
    public Dictionary<string, GameObject> characterPrefabs;
    public Dictionary<string, Sprite> spritesUI;
    public List<RenderTexture> textures;
    public List<Material> templarMaterials;
    public List<Material> valkyrieMaterials;
    public List<Material> pirateMaterials;
    public List<Material> blackKnightMaterials;
    public ArcadeController arcadeInstance;
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }


    void Awake () {
        if (instance != null && instance != this)
        {
            DestroyImmediate(this.gameObject);
        }else
        {
            instance = this;
            loadCharacters();
            loadMaps();
            loadUISprites();
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += checkScene;
        }        
    }
    /// <summary>
    /// Loads all the maps of the game for future Instantiation.
    /// </summary>
    private void loadMaps()
    {
        mapPrefabs = new Dictionary<string, GameObject>();
        mapPrefabs.Add("PirateMap", Resources.Load<GameObject>("Maps/PirateMap"));
        mapPrefabs.Add("ValkyrieMap", Resources.Load<GameObject>("Maps/ValkyrieMap"));
        mapPrefabs.Add("TemplarMap", Resources.Load<GameObject>("Maps/TemplarMap"));
        mapPrefabs.Add("SamuraiMap", Resources.Load<GameObject>("Maps/SamuraiMap"));
        mapPrefabs.Add("BaseMap", Resources.Load<GameObject>("Maps/BaseMap"));
    }
    /// <summary>
    /// Loads all character for future Instantiation.
    /// </summary>
    private void loadCharacters()
    {
        characterPrefabs = new Dictionary<string, GameObject>();
        characterPrefabs.Add("Templar_Player", Resources.Load<GameObject>("Characters/Templar_Player"));
        characterPrefabs.Add("Templar_AI", Resources.Load<GameObject>("Characters/Templar_AI"));
        characterPrefabs.Add("Valkyrie_Player", Resources.Load<GameObject>("Characters/Valkyrie_Player"));
        characterPrefabs.Add("Valkyrie_AI", Resources.Load<GameObject>("Characters/Valkyrie_AI"));
        characterPrefabs.Add("Pirate_Player", Resources.Load<GameObject>("Characters/Pirate_Player"));
        characterPrefabs.Add("Pirate_AI", Resources.Load<GameObject>("Characters/Pirate_AI"));
        characterPrefabs.Add("BlackTemplar_Player", Resources.Load<GameObject>("Characters/BlackTemplar_Player"));
        characterPrefabs.Add("BlackTemplar_AI", Resources.Load<GameObject>("Characters/BlackTemplar_AI"));
        characterPrefabs.Add("Templar_Training", Resources.Load<GameObject>("Characters/Templar_Training"));
        characterPrefabs.Add("Templar_Boss", Resources.Load<GameObject>("Characters/Templar_Boss"));
    }
    /// <summary>
    /// Loads all the UI Sprites for future Instantiation
    /// </summary>
    private void loadUISprites()
    {
        spritesUI = new Dictionary<string, Sprite>();
        spritesUI.Add("HUDImageTemplar", Resources.Load<Sprite>("UI/Images/HUD/HUDImageTemplar"));
        spritesUI.Add("HUDImageValkyrie", Resources.Load<Sprite>("UI/Images/HUD/HUDImageValkyrie"));
        spritesUI.Add("HUDImagePirate", Resources.Load<Sprite>("UI/Images/HUD/HUDImagePirate"));
        spritesUI.Add("HUDImageBlackTemplar", Resources.Load<Sprite>("UI/Images/HUD/HUDImageBlackTemplar"));
        spritesUI.Add("SelectionImageTemplar", Resources.Load<Sprite>("UI/Images/Selection/TemplarSelection"));
        spritesUI.Add("SelectionImageValkyrie", Resources.Load<Sprite>("UI/Images/Selection/ValkyrieSelection"));
        spritesUI.Add("SelectionImagePirate", Resources.Load<Sprite>("UI/Images/Selection/PirateSelection"));
        spritesUI.Add("SelectionImageBlackTemplar", Resources.Load<Sprite>("UI/Images/Selection/BlackTemplarSelection"));
        spritesUI.Add("BGTemplar", Resources.Load<Sprite>("UI/Images/Selection/SelectionBGTemplar"));
        spritesUI.Add("BGValkyrie", Resources.Load<Sprite>("UI/Images/Selection/SelectionBGValkyrie"));
        spritesUI.Add("BGPirate", Resources.Load<Sprite>("UI/Images/Selection/SelectionBGPirate"));
        spritesUI.Add("BGBlackTemplar", Resources.Load<Sprite>("UI/Images/Selection/SelectionBGBlackTemplar"));
        spritesUI.Add("ScoreImageTemplar", Resources.Load<Sprite>("UI/Images/Score/ScoreImageTemplar"));
        spritesUI.Add("ScoreImageValkyrie", Resources.Load<Sprite>("UI/Images/Score/ScoreImageValkyrie"));
        spritesUI.Add("ScoreImagePirate", Resources.Load<Sprite>("UI/Images/Score/ScoreImagePirate"));
        spritesUI.Add("ScoreImageBlackTemplar", Resources.Load<Sprite>("UI/Images/Score/ScoreImageBlackTemplar"));
        spritesUI.Add("IdentifierP1", Resources.Load<Sprite>("UI/Images/HUD/IdentifierP1"));
        spritesUI.Add("IdentifierP2", Resources.Load<Sprite>("UI/Images/HUD/IdentifierP2"));
        spritesUI.Add("IdentifierP3", Resources.Load<Sprite>("UI/Images/HUD/IdentifierP3"));
        spritesUI.Add("IdentifierP4", Resources.Load<Sprite>("UI/Images/HUD/IdentifierP4"));
        spritesUI.Add("IdentifierCPU", Resources.Load<Sprite>("UI/Images/HUD/IdentifierCPU"));
    }

    /// <summary>
    /// Caleed when a scene is loaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void checkScene(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
        switch (name)
        {
            case "FightScene": {
                    loadFightScene();
                } break;
        }
    }
    /// <summary>
    /// Sets up the selected Fight.
    /// </summary>
    private void loadFightScene()
    {
        string selectedMap = Settings.Instance.fightSettings.mapSelection;
        Vector2 pos = new Vector2(0, 0);
        Instantiate(mapPrefabs[selectedMap], pos, Quaternion.identity);

        string fightType = Settings.Instance.fightSettings.type;
        GameObject fight = new GameObject();
        fight.name = "Fight";
        prepareAudioSources(fight);      
        switch (fightType)
        {
            case "LifeFight": fight.AddComponent<LifeFight>(); break;
            case "WaveFight": fight.AddComponent<WaveFight>(); break;
            case "TeamFight": fight.AddComponent<TeamFight>(); break;
            case "TimedFight": fight.AddComponent<TimedFight>(); break;
            case "BossFight": fight.AddComponent<BossFight>(); break;
            case "TeamBossFight": fight.AddComponent<TeamBossFight>(); break;
            case "TeamWaveFight": fight.AddComponent<TeamWaveFight>(); break;
            case "TrainingFight": fight.AddComponent<TrainingFight>(); break;
        }
        currentFight = fight.GetComponent<Fight>();
    }

    private void prepareAudioSources(GameObject fight)
    {
        
        for(int i = 0; i < 3; i++)
        {
            fight.AddComponent<AudioSource>();
        }

        AudioSource[] audios = fight.GetComponents<AudioSource>();

        for (int i = 0; i < 3; i++)
        {
            audios[i].playOnAwake = false;
            audios[i].loop = false;
            audios[i].outputAudioMixerGroup = SoundManager.instance.sfx;
        }
    }
}
