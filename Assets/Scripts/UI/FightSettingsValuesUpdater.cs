using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightSettingsValuesUpdater : MonoBehaviour {

    public GameObject[] texts;
    private void Update()
    {
        UpdatePanelValues();
    }
    // Use this for initialization
    public void UpdatePanelValues()
    {
        texts[0].GetComponent<Text>().text = Settings.Instance.fightSettings.type;
        if (Settings.Instance.fightSettings.type == "LifeFight" || Settings.Instance.fightSettings.type == "TeamFight")
        {
            texts[1].GetComponent<Text>().text = "Lifes: " + Settings.Instance.fightSettings.winFactorQ.ToString();
        }
        else texts[1].GetComponent<Text>().text = "Time: " + Settings.Instance.fightSettings.winFactorQ.ToString();
    }
}
