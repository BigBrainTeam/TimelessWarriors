using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightSettingsController : MonoBehaviour {

    public string fightType, stageChoice;
    public int lifes, time;
    enum FightType { LifeFight, TeamFight, TimedFight}
    FightType type;

    GameObject[] buttons;

    void Start()
    {
        lifes = Settings.Instance.fightSettings.winFactorQ;
        time = Settings.Instance.fightSettings.winFactorQ;
        fightType = Settings.Instance.fightSettings.type;
        stageChoice = Settings.Instance.fightSettings.stageChoice;
        buttons = new GameObject[transform.childCount];      
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons[i] = transform.GetChild(i).gameObject;
        }

        switch (fightType)
        {
            case "LifeFight": setLifeState(); break;
            case "TimedFight": setTimedState(); break;
            case "TeamFight": setTeamState(); break;
        }
    }

    public void changeState()
    {
        switch (fightType)
        {
            case "LifeFight": setTimedState(); break;
            case "TimedFight": setTeamState(); break;
            case "TeamFight": setLifeState(); break;
        }
    }


    void increaseTime()
    {
        switch (time)
        {
            case 1: time = 3; break;
            case 3: time = 5; break;
            case 5: time = 10; break;
            case 10: time = 1; break;
        }
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = time.ToString() + " min.";
    }
    void decreaseTime()
    {
        switch (time)
        {
            case 1: time = 10; break;
            case 3: time = 1; break;
            case 5: time = 3; break;
            case 10: time = 5; break;
        }
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = time.ToString() + " min.";
    }

    void increaseLifes()
    {
        if (lifes < 99) lifes++;
        else lifes = 1;
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = lifes.ToString();
    }
    void decreaseLifes()
    {
        if (lifes < 2) lifes = 99;
        else lifes--;
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = lifes.ToString();
    }

    public void increaseFactor()
    {
        if (fightType == "LifeFight" || fightType == "TeamFight") increaseLifes();
        else increaseTime();
    }

    public void decreaseFactor()
    {
        if (fightType == "LifeFight" || fightType == "TeamFight") decreaseLifes();
        else decreaseTime();
    }

    public void changeStageChoice()
    {
        if (stageChoice == "CHOOSE") stageChoice = "RANDOM";
        else stageChoice = "CHOOSE";
        buttons[3].transform.GetChild(1).GetComponentInChildren<Text>().text = stageChoice;
    }

    private void setTimedState()
    {
        buttons[1].transform.GetChild(1).GetComponentInChildren<Text>().text = "TIMED FIGHT";
        buttons[2].transform.GetChild(0).GetComponent<Text>().text = "TIME";
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = time.ToString()+" min.";
        fightType = "TimedFight";
        type = FightType.TimedFight;
        //foreach (MenuPlayerController mpc in MenuManager.instance.players)
        //{
        //    if (mpc.joined) mpc.hasSelectedColor = false;
        //}
    }

    private void setLifeState()
    {
        buttons[1].transform.GetChild(1).GetComponentInChildren<Text>().text = "LIFE FIGHT";
        buttons[2].transform.GetChild(0).GetComponent<Text>().text = "LIFES";
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = lifes.ToString();
        fightType = "LifeFight";
        type = FightType.LifeFight;
        //foreach (MenuPlayerController mpc in MenuManager.instance.players)
        //{
        //    if (mpc.joined) mpc.hasSelectedColor = false;
        //}
    }

    private void setTeamState()
    {
        buttons[1].transform.GetChild(1).GetComponentInChildren<Text>().text = "TEAM FIGHT";
        buttons[2].transform.GetChild(0).GetComponent<Text>().text = "LIFES";
        buttons[2].transform.GetChild(1).GetComponentInChildren<Text>().text = lifes.ToString();
        fightType = "TeamFight";
        type = FightType.TeamFight;
        //foreach(MenuPlayerController mpc in MenuManager.instance.players)
        //{
        //   if(mpc.joined) mpc.hasSelectedColor = true;
        //}
    }

    public void saveCombatSettings()
    {
        Settings.Instance.fightSettings.type = fightType;
        if (fightType == "LifeFight" || fightType == "TeamFight")
        {
            Settings.Instance.fightSettings.winFactorQ = lifes;
        }
        else Settings.Instance.fightSettings.winFactorQ = time;
        Settings.Instance.fightSettings.stageChoice = stageChoice;
        Settings.Instance.SaveSettings("fight");
    }
}
