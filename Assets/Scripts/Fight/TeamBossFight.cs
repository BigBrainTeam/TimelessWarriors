using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamBossFight : TeamFight
{
    List<int> currentLife;
    List<Text> lifeText;
    Entity[] team2;
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
                Text text = playerBars.GetChild(i).transform.GetChild(4).GetComponentInChildren<Text>();
                text.text = maxLife.ToString();
                lifeText.Add(text);
                currentLife.Add(maxLife);
        }
        
    }
    public override void initTeams()
    {
        team1 = new Entity[2];
        team2 = new Entity[1];
        int totalPlayers = players.Count;
        for (int i = 0; i < totalPlayers; i++)
        {
            string characterSelection = Settings.Instance.fightSettings.selectedCharacters[i];
            string[] split = characterSelection.Split('_');
            if (i < 2)
            {
                switch (split[0])
                {
                    case "Templar":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.templarMaterials[1];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.templarMaterials[1];
                        }
                        break;
                    case "Valkyrie":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.valkyrieMaterials[1];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[1];
                        }
                        break;
                    case "Pirate":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.pirateMaterials[1];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[1];
                        }
                        break;
                    case "BlackTemplar":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.blackKnightMaterials[1];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[1];
                        }
                        break;
                }
                team1[i] = players[i];
            }
            else
            {
                switch (split[0])
                {
                    case "Templar":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.templarMaterials[3];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.templarMaterials[3];
                        }
                        break;
                    case "Valkyrie":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.valkyrieMaterials[3];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[3];
                        }
                        break;
                    case "Pirate":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.pirateMaterials[3];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[3];
                        }
                        break;
                    case "BlackTemplar":
                        {
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.blackKnightMaterials[3];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[3];
                        }
                        break;
                }
                team2[i - 2] = players[i];
            }
        }

        Physics2D.IgnoreCollision(players[0].transform.GetChild(2).GetComponent<Collider2D>(), players[1].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[1].transform.GetChild(2).GetComponent<Collider2D>(), players[0].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[2].transform.GetChild(2).GetComponent<Collider2D>(), players[3].transform.GetChild(3).GetComponent<Collider2D>());
    }

    public override void onBorderCollision(Entity ent)
    {
        base.onBorderCollision(ent);
        if (ent is Character)
        {
            killCharacter((Character)ent);
        }
    }
    public override bool fightIsOver()
    {
        int deaths = 0;
        if (team1 != null)
        {
            foreach (Entity e in team1)
            {
                if (e.isDead) deaths++;
            }
        }
        if (deaths == 2)
        {
            winner = 2;
            return true;
        }

        deaths = 0;
        if (team2 != null)
        {
            foreach (Entity e in team2)
            {
                if (e.isDead) deaths++;
            }
        }
        if (deaths == 1)
        {
            winner = 1;
            return true;
        }

        return false;
    }
}
