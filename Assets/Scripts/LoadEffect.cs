using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loading effect class.
/// </summary>
public class LoadEffect : MonoBehaviour {
    public GameObject[] players;
    public Image loadBar;
	// Use this for initialization
	void Start () {
        int totalPlayers = Settings.Instance.fightSettings.selectedCharacters.Count;
        for (int i = 0; i< totalPlayers; i++)
        {
            players[i].SetActive(true);
            string[] split = Settings.Instance.fightSettings.selectedCharacters[i].Split('_');
            string name = split[0];
            players[i].GetComponent<Image>().sprite = GameManager.Instance.spritesUI["BG" + name];
            players[i].transform.GetChild(1).GetComponent<Image>().sprite =  GameManager.Instance.spritesUI["SelectionImage" + name];
            switch (name)
            {
                case "Templar":
                    {
                        players[i].transform.GetComponentInChildren<Text>().text = "Lancelot";
                        players[i].transform.GetChild(1).GetComponent<Image>().material = GameManager.Instance.templarMaterials[Settings.Instance.fightSettings.selectedSkins[i]];
                    } break;
                case "Valkyrie":
                    {
                        players[i].transform.GetComponentInChildren<Text>().text = "Kara";
                        players[i].transform.GetChild(1).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[Settings.Instance.fightSettings.selectedSkins[i]];
                    }
                    break;
                case "Pirate":
                    {
                        players[i].transform.GetComponentInChildren<Text>().text = "Blackbeard";
                        players[i].transform.GetChild(1).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[Settings.Instance.fightSettings.selectedSkins[i]];
                    }
                    break;
                case "BlackTemplar":
                    {
                        players[i].transform.GetComponentInChildren<Text>().text = "Endo";
                        players[i].transform.GetChild(1).GetComponent<Image>().material = GameManager.Instance.blackKnightMaterials[Settings.Instance.fightSettings.selectedSkins[i]];
                    }
                    break;
            }
        }
        StartCoroutine(falseLoading());
	}
    /// <summary>
    /// Fake loading bar
    /// </summary>
    /// <returns></returns>
    private IEnumerator falseLoading()
    {
        float fill = 0;
        while (fill < 1)
        {
            fill+= 0.05f;
            loadBar.fillAmount = fill;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.currentFight.StartCoroutine(GameManager.Instance.currentFight.countdown());
        Destroy(this.gameObject);
    }
}
