using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossFight : Fight
{
    List<int> currentLife;
    List<Text> lifeText;

    private void Awake()
    {
        base.Awake();
        
    }
    override protected void init()
    {
        base.init();
        initLifes();
    }

    private void initLifes()
    {
        lifeText = new List<Text>();
        currentLife = new List<int>();
        int maxLife = Settings.Instance.fightSettings.winFactorQ;
        Transform playerBars = GameObject.Find("PlayerBars").transform;
        for (int i = 0; i < players.Count; i++)
        {
            if (i == 0)
            {
                Text text = playerBars.GetChild(i).transform.GetChild(4).GetComponentInChildren<Text>();
                text.text = maxLife.ToString();
                lifeText.Add(text);
                currentLife.Add(maxLife);
            }
            if (i == 1)
            {
                Text text = playerBars.GetChild(i).transform.GetChild(4).GetComponentInChildren<Text>();
                text.text = "1";
                lifeText.Add(text);
                currentLife.Add(1);
            }
        }
        
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
            base.killCharacter(ent);
            int currentPlayer = ent.player;
            currentLife[currentPlayer]--;
            lifeText[currentPlayer].text = currentLife[currentPlayer].ToString();

            if (currentLife[currentPlayer] <= 0) finishPlayer(currentPlayer);
            else StartCoroutine(respawnCharacter((Character)ent, currentPlayer));
        }
    }
}
