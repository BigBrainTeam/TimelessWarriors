using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeController : MonoBehaviour {

    public int combat = 0;
    int actualCombat = -1;
    int players;
    static public bool next = true;
    static public bool finish = false;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
    }
	// Update is called once per frame
	void Update () { 
        if(actualCombat != combat)
        {
            initRound();
            actualCombat = combat;
        }
        if (next) { combat++; next = false; }
    }
    public void initRound()
    {
        players = MenuManager.instance.currentCharacters;
        if (players == 1)
        {
            if (combat == 1)
            {
                Settings.Instance.fightSettings.mapSelection = "ValkyrieMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Valkyrie_AI");
                Settings.Instance.fightSettings.selectedSkins.Add(0);
                Settings.Instance.fightSettings.type = "LifeFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 2)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Valkyrie_AI");
                Settings.Instance.fightSettings.selectedSkins.RemoveAt(1);
                Settings.Instance.fightSettings.mapSelection = "BaseMap";
                Settings.Instance.fightSettings.type = "WaveFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 3)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.mapSelection = "TemplarMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_AI");
                Settings.Instance.fightSettings.selectedSkins.Add(1);
                Settings.Instance.fightSettings.type = "LifeFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 4)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.mapSelection = "BaseMap";
                Settings.Instance.fightSettings.selectedCharacters.Remove("Templar_AI");
                Settings.Instance.fightSettings.selectedSkins.RemoveAt(1);
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_Boss");
                Settings.Instance.fightSettings.selectedSkins.Add(1);
                Settings.Instance.fightSettings.type = "BossFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 5)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Templar_Boss");
                Settings.Instance.fightSettings.mapSelection = "PirateMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Pirate_AI");
                Settings.Instance.fightSettings.selectedSkins.Add(3);
                Settings.Instance.fightSettings.type = "LifeFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 6)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Pirate_AI");
                Settings.Instance.fightSettings.mapSelection = "SamuraiMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("BlackTemplar_AI");
                Settings.Instance.fightSettings.selectedSkins.Add(3);
                Settings.Instance.fightSettings.type = "LifeFight";
                SceneManager.LoadScene("FightScene");
                finish = true;
            }
        }
        else
        {
            if (combat == 1)
            {
                if(MenuManager.instance.currentPlayers == 1) { Settings.Instance.fightSettings.selectedCharacters.Add(MenuManager.instance.CPUs[0]); Settings.Instance.fightSettings.selectedSkins.Add(0);}
                Settings.Instance.fightSettings.mapSelection = "ValkyrieMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Valkyrie_AI");
                Settings.Instance.fightSettings.selectedCharacters.Add("Valkyrie_AI");
                Settings.Instance.fightSettings.selectedSkins.Add(0);
                Settings.Instance.fightSettings.selectedSkins.Add(0);
                Settings.Instance.fightSettings.type = "TeamFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 2)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Valkyrie_AI");
                Settings.Instance.fightSettings.selectedCharacters.Remove("Valkyrie_AI");
                Settings.Instance.fightSettings.mapSelection = "BaseMap";
                Settings.Instance.fightSettings.type = "TeamWaveFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 3)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.mapSelection = "TemplarMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_AI");
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_AI");
                Settings.Instance.fightSettings.type = "TeamFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 4)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Templar_AI");
                Settings.Instance.fightSettings.selectedCharacters.Remove("Templar_AI");
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_Boss");
                Settings.Instance.fightSettings.mapSelection = "BaseMap";
                Settings.Instance.fightSettings.type = "TeamBossFight";
                SceneManager.LoadScene("FightScene");
            }
            else if (combat == 5)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Templar_Boss");
                Settings.Instance.fightSettings.mapSelection = "PirateMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("Pirate_AI");
                Settings.Instance.fightSettings.selectedCharacters.Add("Pirate_AI");
                Settings.Instance.fightSettings.type = "TeamFight";
                SceneManager.LoadScene("FightScene");
                finish = true;
            }
            else if (combat == 6)
            {
                Time.timeScale = 1f;
                Settings.Instance.fightSettings.selectedCharacters.Remove("Pirate_AI");
                Settings.Instance.fightSettings.selectedCharacters.Remove("Pirate_AI");
                Settings.Instance.fightSettings.mapSelection = "SamuraiMap";
                Settings.Instance.fightSettings.selectedCharacters.Add("BlackTemplar_AI");
                Settings.Instance.fightSettings.selectedCharacters.Add("BlackTemplar_AI");
                Settings.Instance.fightSettings.type = "TeamFight";
                SceneManager.LoadScene("FightScene");
                finish = true;
            }
        }
    }
}
