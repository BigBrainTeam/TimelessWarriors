using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimedFight : Fight
{

    float timer;
    List<Text> pointsText;
    public GameObject timePrefab;
    GameObject timeInstance;
    Text timeText;
    Vector3 timerPosition;

    // Use this for initialization
    override protected void Awake()
    {
        statisticNames = new List<string> { "DAMAGEDONE", "DAMAGERECEIVED", "POINTS", "PERFECTS", "KILLSPREE", "DEATHS", "FALLS", "ULTIMATES" };
        base.Awake();
    }
    protected override void init()
    {
        base.init();
        timerPosition = new Vector3(0, 150, 0);
        timer = Settings.Instance.fightSettings.winFactorQ * 60;
        initPoints();
        timeInstance = GameObject.Instantiate(Resources.Load<GameObject>("UI/Time"));
        timeInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        timeInstance.transform.SetSiblingIndex(3);
        timeText = timeInstance.GetComponentInChildren<Text>();

        float minutes = Mathf.Floor(timer / 60);
        float seconds = Mathf.Floor(((timer / 60) - minutes) * 60);
        string secondsText;
        if (seconds < 10) secondsText = "0" + seconds.ToString();
        else secondsText = seconds.ToString();
        timeText.text = minutes.ToString() + ":" + secondsText;
    }

    protected override void startFight()
    {
        base.startFight();
        StartCoroutine(fightTimer());
    }
    private void initPoints()
    {
        pointsText = new List<Text>();
        Transform playerBars = GameObject.Find("PlayerBars").transform;
        for (int i = 0; i < players.Count; i++)
        {
            Text text = playerBars.GetChild(i).transform.GetChild(4).GetComponentInChildren<Text>();
            text.text = "0";
            pointsText.Add(text);
        }
    }
    /// <summary>
    /// Timer for the Fight.
    /// </summary>
    /// <returns></returns>
    private IEnumerator fightTimer()
    {
        while (timer > 0)
        {
            float minutes = Mathf.Floor(timer / 60);
            float seconds = Mathf.Floor(((timer / 60) - minutes) * 60);
            string secondsText;
            if (seconds < 10) secondsText = "0" + seconds.ToString();
            else secondsText = seconds.ToString();
            timeText.text = minutes.ToString() + ":" + secondsText;
            yield return new WaitForSeconds(1);
            timer--;
        }
        timeText.text = "0:00";
        finishFight();
    }

    public override void onBorderCollision(Entity ent)
    {
        base.onBorderCollision(ent);
        if (ent is Character)
        {
            killCharacter((Character)ent);
        }
    }

    protected override void killCharacter(Character ent)
    {
        if (!ent.isDead)
        {
            ent.enabled = false;
            ent.isDead = true;
            ent.RigidBody.bodyType = RigidbodyType2D.Kinematic;
            ent.setLife(0);
            ent.Animator.Play("Falling");
            if (ent.state == Utilities.state.Attacking || ent.state == Utilities.state.Ultimate) ent.stopAttacking();

            int currentPlayer = ent.player;
            int lasthitter = ent.hittedBy;

            onPlayerInsideCamera(currentPlayer);
            if (lasthitter >= 0)
            {
                Entity lasthitterEntity = players[lasthitter].GetComponent<Entity>();
                if (lasthitterEntity.getLife() == lasthitterEntity.getMaxLife()) statistics[(int)Utilities.statistic.PERFECTS][lasthitter]++;
                statistics[(int)Utilities.statistic.KOS][lasthitter] += 2;
                statistics[(int)Utilities.statistic.CURRENTSPREE][lasthitter]++;
                statistics[(int)Utilities.statistic.DEATHS][currentPlayer]++;
                statistics[(int)Utilities.statistic.KOS][currentPlayer]--;
                pointsText[lasthitter].text = statistics[(int)Utilities.statistic.KOS][lasthitter].ToString();
            }
            else
            {
                statistics[(int)Utilities.statistic.FALLS][currentPlayer]++;
                statistics[(int)Utilities.statistic.KOS][currentPlayer] -= 2;
            }

            int currentSpree = statistics[(int)Utilities.statistic.CURRENTSPREE][currentPlayer];
            if (currentSpree > statistics[(int)Utilities.statistic.KILLSPREE][currentPlayer]) statistics[(int)Utilities.statistic.KILLSPREE][currentPlayer] = currentSpree;
            statistics[(int)Utilities.statistic.CURRENTSPREE][currentPlayer] = 0;

            pointsText[currentPlayer].text = statistics[(int)Utilities.statistic.KOS][currentPlayer].ToString();

            StartCoroutine(respawnCharacter((Character)ent, currentPlayer));
        }
    }

    override protected void finishFight()
    {
        Time.timeScale = 0.2f;
        finishImage.gameObject.SetActive(true);
        finishImage.DOScale(0.5f, 0.3f);


        int[] pointValues = new int[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            pointValues[i] = statistics[(int)Utilities.statistic.KOS][i];
        }

        switch (players.Count)
        {
            case 2:
                {
                    if (pointValues[0] >= pointValues[1])
                    {
                        if (pointValues[0] == pointValues[1])
                        {
                            statistics[(int)Utilities.statistic.RANK][0] = 1;
                            statistics[(int)Utilities.statistic.RANK][1] = 1;
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][0] = 1;
                            statistics[(int)Utilities.statistic.RANK][1] = 2;
                        }
                    }
                    else
                    {
                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                        statistics[(int)Utilities.statistic.RANK][1] = 1;
                    }
                }
                break;
            case 3:
                {
                    if (pointValues[0] >= pointValues[1] || pointValues[0] >= pointValues[2])
                    {
                        if(pointValues[0] == pointValues[1] || pointValues[0] == pointValues[2])
                        {
                            if (pointValues[0] == pointValues[1])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                            }
                            if (pointValues[0] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                            }
                            if(pointValues[0] == pointValues[1] && pointValues[0] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                            }
                        }                 
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][0] = 1;
                            if (pointValues[1] >= pointValues[2])
                            {
                                if (pointValues[1] == pointValues[2])
                                {
                                    statistics[(int)Utilities.statistic.RANK][1] = 2;
                                    statistics[(int)Utilities.statistic.RANK][2] = 2;
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][1] = 2;
                                    statistics[(int)Utilities.statistic.RANK][2] = 3;
                                }
                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][1] = 3;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                            }
                        }
                    }
                    else if (pointValues[1] >= pointValues[0] || pointValues[1] >= pointValues[2])
                    {
                        if (pointValues[0] == pointValues[1] || pointValues[1] == pointValues[2])
                        {
                            if (pointValues[0] == pointValues[1])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                            }
                            if (pointValues[1] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                            }
                            if (pointValues[0] == pointValues[1] && pointValues[1] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                            }
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][1] = 1;
                            if (pointValues[0] >= pointValues[2])
                            {
                                if (pointValues[0] == pointValues[2])
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    statistics[(int)Utilities.statistic.RANK][2] = 2;
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    statistics[(int)Utilities.statistic.RANK][2] = 3;
                                }
                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 3;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                            }
                        }
                    }
                    else if (pointValues[2] >= pointValues[0] || pointValues[2] >= pointValues[1])
                    {
                        if (pointValues[0] == pointValues[2] || pointValues[1] == pointValues[2])
                        {
                            if (pointValues[0] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                            }
                            if (pointValues[1] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                            }
                            if (pointValues[0] == pointValues[2] && pointValues[1] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                            }
                        }  
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][2] = 1;
                            if (pointValues[0] >= pointValues[1])
                            {
                                if (pointValues[0] == pointValues[1])
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    statistics[(int)Utilities.statistic.RANK][1] = 2;
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    statistics[(int)Utilities.statistic.RANK][1] = 3;
                                }
                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 3;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                            }
                        }
                    }

                }
                break;
            case 4:
                {
                    if (pointValues[0] >= pointValues[1] || pointValues[0] >= pointValues[2] || pointValues[0] == pointValues[3])
                    {
                        if (pointValues[0] == pointValues[1] || pointValues[0] == pointValues[2] || pointValues[0] == pointValues[3])
                        {
                            if (pointValues[0] == pointValues[1])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[0] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[0] == pointValues[3])
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                            }
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][0] = 1;
                            if (pointValues[1] >= pointValues[2] || pointValues[1] >= pointValues[3])
                            {
                                if (pointValues[1] == pointValues[2] || pointValues[1] == pointValues[3])
                                {
                                    if (pointValues[1] == pointValues[2])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                    if (pointValues[1] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 3;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                    if (pointValues[1] == pointValues[2] && pointValues[1] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][1] = 2;
                                    if (pointValues[2] >= pointValues[3])
                                    {
                                        if (pointValues[2] == pointValues[3])
                                        {
                                            statistics[(int)Utilities.statistic.RANK][2] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 3;
                                        }
                                        else
                                        {
                                            statistics[(int)Utilities.statistic.RANK][2] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 4;
                                        }
                                    }
                                    else
                                    {
                                        statistics[(int)Utilities.statistic.RANK][2] = 4;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                }

                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                                statistics[(int)Utilities.statistic.RANK][3] = 4;
                            }
                        }
                    }
                    else if (pointValues[1] >= pointValues[0] || pointValues[1] >= pointValues[2] || pointValues[1] == pointValues[3])
                    {
                        if (pointValues[1] == pointValues[0] || pointValues[1] == pointValues[2] || pointValues[1] == pointValues[3])
                        {
                            if (pointValues[1] == pointValues[0])
                            {
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[1] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[1] == pointValues[3])
                            {
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                            }
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][1] = 1;
                            if (pointValues[0] >= pointValues[2] || pointValues[0] >= pointValues[3])
                            {
                                if (pointValues[0] == pointValues[2] || pointValues[0] == pointValues[3])
                                {
                                    if (pointValues[0] == pointValues[2])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                    if (pointValues[0] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 3;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                    if (pointValues[0] == pointValues[2] && pointValues[0] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    if (pointValues[2] >= pointValues[3])
                                    {
                                        if (pointValues[2] == pointValues[3])
                                        {
                                            statistics[(int)Utilities.statistic.RANK][2] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 3;
                                        }
                                        else
                                        {
                                            statistics[(int)Utilities.statistic.RANK][2] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 4;
                                        }
                                    }
                                    else
                                    {
                                        statistics[(int)Utilities.statistic.RANK][2] = 4;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                }

                            }else
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                                statistics[(int)Utilities.statistic.RANK][3] = 4;
                            }
                        }
                    }
                    else if (pointValues[2] >= pointValues[0] || pointValues[2] >= pointValues[1] || pointValues[2] == pointValues[3])
                    {
                        if (pointValues[2] == pointValues[0] || pointValues[2] == pointValues[1] || pointValues[2] == pointValues[3])
                        {
                            if (pointValues[2] == pointValues[0])
                            {
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[2] == pointValues[1])
                            {
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][3] = 3;
                            }
                            if (pointValues[2] == pointValues[3])
                            {
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][1] = 3;
                            }
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][2] = 1;
                            if (pointValues[0] >= pointValues[1] || pointValues[0] >= pointValues[3])
                            {
                                if (pointValues[0] == pointValues[1] || pointValues[0] == pointValues[3])
                                {
                                    if (pointValues[0] == pointValues[1])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                    if (pointValues[0] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 3;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                    if (pointValues[0] == pointValues[1] && pointValues[0] == pointValues[3])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][3] = 2;
                                    }
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    if (pointValues[1] >= pointValues[3])
                                    {
                                        if (pointValues[1] == pointValues[3])
                                        {
                                            statistics[(int)Utilities.statistic.RANK][1] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 3;
                                        }
                                        else
                                        {
                                            statistics[(int)Utilities.statistic.RANK][1] = 3;
                                            statistics[(int)Utilities.statistic.RANK][3] = 4;
                                        }
                                    }
                                    else
                                    {
                                        statistics[(int)Utilities.statistic.RANK][1] = 4;
                                        statistics[(int)Utilities.statistic.RANK][3] = 3;
                                    }
                                }

                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][1] = 3;
                                statistics[(int)Utilities.statistic.RANK][3] = 4;
                            }
                        }
                    }
                    else if (pointValues[3] >= pointValues[0] || pointValues[3] >= pointValues[1] || pointValues[3] == pointValues[2])
                    {
                        if (pointValues[3] == pointValues[0] || pointValues[3] == pointValues[1] || pointValues[3] == pointValues[2])
                        {
                            if (pointValues[3] == pointValues[0])
                            {
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                            }
                            if (pointValues[3] == pointValues[1])
                            {
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][1] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][2] = 3;
                            }
                            if (pointValues[3] == pointValues[2])
                            {
                                statistics[(int)Utilities.statistic.RANK][3] = 1;
                                statistics[(int)Utilities.statistic.RANK][2] = 1;
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][1] = 3;
                            }
                        }
                        else
                        {
                            statistics[(int)Utilities.statistic.RANK][3] = 1;
                            if (pointValues[0] >= pointValues[1] || pointValues[0] >= pointValues[2])
                            {
                                if (pointValues[0] == pointValues[1] || pointValues[0] == pointValues[2])
                                {
                                    if (pointValues[0] == pointValues[1])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 3;
                                    }
                                    if (pointValues[0] == pointValues[2])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 3;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                    }
                                    if (pointValues[0] == pointValues[1] && pointValues[0] == pointValues[2])
                                    {
                                        statistics[(int)Utilities.statistic.RANK][0] = 2;
                                        statistics[(int)Utilities.statistic.RANK][1] = 2;
                                        statistics[(int)Utilities.statistic.RANK][2] = 2;
                                    }
                                }
                                else
                                {
                                    statistics[(int)Utilities.statistic.RANK][0] = 2;
                                    if (pointValues[1] >= pointValues[2])
                                    {
                                        if (pointValues[1] == pointValues[2])
                                        {
                                            statistics[(int)Utilities.statistic.RANK][1] = 3;
                                            statistics[(int)Utilities.statistic.RANK][2] = 3;
                                        }
                                        else
                                        {
                                            statistics[(int)Utilities.statistic.RANK][1] = 3;
                                            statistics[(int)Utilities.statistic.RANK][2] = 4;
                                        }
                                    }
                                    else
                                    {
                                        statistics[(int)Utilities.statistic.RANK][1] = 4;
                                        statistics[(int)Utilities.statistic.RANK][2] = 3;
                                    }
                                }

                            }
                            else
                            {
                                statistics[(int)Utilities.statistic.RANK][0] = 2;
                                statistics[(int)Utilities.statistic.RANK][1] = 3;
                                statistics[(int)Utilities.statistic.RANK][2] = 4;
                            }
                        }
                    }
                }
                break;
        }
        Invoke("swapScene", 25f * Time.unscaledDeltaTime);
    }
}
