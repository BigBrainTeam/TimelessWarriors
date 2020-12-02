using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class ScoreScreen : MonoBehaviour {

    Fight fight;
    public GameObject[] panels;
    Sequence valueEffect;
    public Sprite[] letters, ranks;
    public GameObject namePrefab, numberPrefab;
    bool next = false;

	// Use this for initialization
	void Start () {
        fight = GameObject.Find("Fight").GetComponent<Fight>();
        prepareScreen();
    }
    void Update()
    {
        if (Input.GetButtonDown("JumpP1") && next)
        {
            if (GameObject.Find("ArcadeManager(Clone)"))
            {
                ArcadeController.next = true;
                if (ArcadeController.finish)
                {
                    Settings.Instance.fightSettings.nPlayersCanJoin = 4;
                    SceneManager.LoadScene("MainMenu");
                }
            }
            else
            {
                SceneManager.LoadScene("CharacterSelection");
                MenuManager.instance.initPlayers();
            }
        }
    }
    void prepareScreen()
    {
        int totalPlayers = Settings.Instance.fightSettings.selectedCharacters.Count;
        for(int i = 0; i<totalPlayers; i++)
        {
            panels[i].SetActive(true);
            showCharacterImage(i);
            StartCoroutine(preparePanel(i));
        }
        fight.StopAllCoroutines();
        Destroy(fight.gameObject);
    }

    private IEnumerator preparePanel(int player)
    {
        panels[player].GetComponent<Image>().color = Settings.Instance.fightSettings.colors[player];
        RectTransform paneltransf = panels[player].GetComponent<RectTransform>();
        valueEffect = DOTween.Sequence();
        float x = 0;
        switch (player)
        {
            case 0: x = -295; break;
            case 1: x = -98; break;
            case 2: x = 100; break;
            case 3: x = 297; break;
        }
        Vector3 startPos = new Vector3(x, -45, 0);
        valueEffect.Append(paneltransf.DOScale(1f, 0.4f));
        valueEffect.Append(paneltransf.DOLocalMove(startPos, 0.4f, false));

        float y = 0;
        int rank = fight.statistics[(int)Utilities.statistic.RANK][player];
        switch (rank)
        {
            case 1: y = 7; break;
            case 2: y = -9; break;
            case 3: y = -27; break;
            case 4: y = -45; break;
        }
        Vector3 finalPos = new Vector3(x, y, 0);
        valueEffect.Append(paneltransf.DOLocalMove(finalPos, 0.5f, false));
        yield return new WaitForSeconds(0.5f);

        panels[player].transform.GetChild(3).gameObject.SetActive(true);
        panels[player].transform.GetChild(3).GetComponent<Image>().sprite = ranks[rank-1];

        StartCoroutine(showPlayerStatistics(player));
    }
    IEnumerator showPlayerStatistics(int player)
    {
        int totalStatistics = GameManager.Instance.currentFight.statisticNames.Count;
        for(int i = 0; i < totalStatistics; i++)
        {
            string statname = fight.statisticNames[i];
            int statvalue = fight.statistics[i][player];

            GameObject nameGO = Instantiate(namePrefab, panels[player].transform.GetChild(1).transform, false) as GameObject;
            GameObject valueGO = Instantiate(numberPrefab, panels[player].transform.GetChild(1).transform, false) as GameObject;

            nameGO.GetComponent<Text>().DOText(statname, 0.5f, true);
            yield return new WaitForSeconds(0.5f);
            initValueSequence(valueGO);
            StartCoroutine(incrementValue(statvalue, valueGO.GetComponent<Text>()));
        }
        showPlayerScore(player);
    }
    void showCharacterImage(int player)
    {
        string selectedCharacter = Settings.Instance.fightSettings.selectedCharacters[player];
        string[] split = selectedCharacter.Split('_');
        panels[player].transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Instance.spritesUI["ScoreImage" + split[0]];
    }

    void initValueSequence(GameObject value)
    {
        valueEffect.Append(value.GetComponent<RectTransform>().DOScale(1f, 0.2f));
    }

    IEnumerator incrementValue(int finalValue, Text text)
    {
        int initValue = 0;
        float waitValue = 0;
        text.text = initValue.ToString();
        while (initValue < finalValue)
        {
            initValue++;
            text.text = initValue.ToString();
            if (initValue < (finalValue * 0.25)) waitValue = 0.00001f;
            else if (initValue < (finalValue * 0.50)) waitValue = 0.00002f;
            else if (initValue < (finalValue * 0.60)) waitValue = 0.0004f;
            else if (initValue < (finalValue * 0.70)) waitValue = 0.0003f;
            else if (initValue < (finalValue * 0.80)) waitValue = 0.0002f;
            else if (initValue < (finalValue * 0.99)) waitValue = 0.01f;
            yield return new WaitForSeconds(waitValue);
        }
    }

    int getScoreRank(int player)
    {
        int kills = fight.statistics[(int)Utilities.statistic.KOS][player];
        int deaths = fight.statistics[(int)Utilities.statistic.DEATHS][player];
        int killspree = fight.statistics[(int)Utilities.statistic.KILLSPREE][player];
        int ults = fight.statistics[(int)Utilities.statistic.ULTIMATES][player];
        int falls = fight.statistics[(int)Utilities.statistic.FALLS][player];
        int perfects = fight.statistics[(int)Utilities.statistic.PERFECTS][player];

        int totalPlayers = Settings.Instance.fightSettings.selectedCharacters.Count; 

        kills *= 200;
        deaths *= -25;
        killspree *= 300;
        ults *= 100;
        falls *= -100;
        perfects *= 900;

        int totalScore = kills + deaths + killspree + ults + falls + perfects;

        if (totalPlayers <= 2)
        {
            if (totalScore < 500) return 4;
            else if (totalScore < 700) return 3;
            else if (totalScore < 1000) return 2;
            else if (totalScore < 1200) return 1;
            else return 0;
        }
        else
        {
            if (totalScore < 1000) return 4;
            else if (totalScore < 1250) return 3;
            else if (totalScore < 1500) return 2;
            else if (totalScore < 3000) return 1;
            else return 0;
        }
    }

    void showPlayerScore(int player)
    {
        int letter = getScoreRank(player);
        panels[player].transform.GetChild(2).gameObject.SetActive(true);
        panels[player].transform.GetChild(2).GetComponent<Image>().sprite = letters[letter];
        panels[player].transform.GetChild(2).DORotate(new Vector3(0,0,720), 0.6f, RotateMode.FastBeyond360);
        panels[player].transform.GetChild(2).DOScale(1f, 0.4f);
        next = true;
    }
}
