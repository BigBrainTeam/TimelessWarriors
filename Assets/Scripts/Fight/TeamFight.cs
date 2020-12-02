using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFight : LifeFight {

    protected Entity[] team1;
    Entity[] team2;
    protected int winner = 0;

    // Use this for initialization
    override protected void init()
    {
        base.init();
        initTeams();
    }
    public virtual void initTeams()
    {
        team1 = new Entity[2];
        team2 = new Entity[2];
        int totalPlayers = players.Count;
        for(int i = 0; i<totalPlayers; i++)
        {
            string characterSelection = Settings.Instance.fightSettings.selectedCharacters[i];
            string[] split = characterSelection.Split('_');
            if (i < 2) {
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
                            players[i].GetComponent<SpriteRenderer>().material = GameManager.Instance.blackKnightMaterials[3];
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[3];
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
                team2[i-2] = players[i];
            }
        }

        Physics2D.IgnoreCollision(players[0].transform.GetChild(2).GetComponent<Collider2D>(), players[1].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[1].transform.GetChild(2).GetComponent<Collider2D>(), players[0].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[2].transform.GetChild(2).GetComponent<Collider2D>(), players[3].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[3].transform.GetChild(2).GetComponent<Collider2D>(), players[2].transform.GetChild(3).GetComponent<Collider2D>());
    }

    internal void ignoreTeammate(int player, Collider2D hitbox)
    {
        switch (player)
        {
            case 0: Physics2D.IgnoreCollision(players[1].HurtBox, hitbox); break;
            case 1: Physics2D.IgnoreCollision(players[0].HurtBox, hitbox); break;
            case 2: Physics2D.IgnoreCollision(players[3].HurtBox, hitbox); break;
            case 3: Physics2D.IgnoreCollision(players[2].HurtBox, hitbox); break;
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
        if (deaths == 2) {
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
        if (deaths == 2) {
            winner = 1;
            return true;
        }

        return false;
    }

    protected override void finishFight()
    {
        base.finishFight();
        if(winner == 1)
        {
            statistics[(int)Utilities.statistic.RANK][0] = 1;
            statistics[(int)Utilities.statistic.RANK][1] = 1;
            statistics[(int)Utilities.statistic.RANK][2] = 2;
            statistics[(int)Utilities.statistic.RANK][3] = 2;
        }
        else
        {
            statistics[(int)Utilities.statistic.RANK][0] = 2;
            statistics[(int)Utilities.statistic.RANK][1] = 2;
            statistics[(int)Utilities.statistic.RANK][2] = 1;
            statistics[(int)Utilities.statistic.RANK][3] = 1;
        }
    }

    public Entity getTeammate(int player)
    {
        switch (player)
        {
            case 0: return players[1];
            case 1: return players[0];
            case 2: return players[3];
            case 3: return players[2];
        }
        return null;
    }
}
