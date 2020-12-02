using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeFight : Fight {

    List<int> currentLife;
    List<Text> lifeText;
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
        for(int i = 0; i < players.Count; i++)
        {
            Text text = playerBars.GetChild(i).transform.GetChild(4).GetComponentInChildren<Text>();
            text.text = maxLife.ToString();
            lifeText.Add(text);
            currentLife.Add(maxLife);
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
        if (!ent.isDead) {
            base.killCharacter(ent);
            int currentPlayer = ent.player;
            currentLife[currentPlayer]--;
            lifeText[currentPlayer].text = currentLife[currentPlayer].ToString();

            if (currentLife[currentPlayer] <= 0) finishPlayer(currentPlayer);
            else StartCoroutine(respawnCharacter((Character)ent, currentPlayer));
        }      
    }
}
