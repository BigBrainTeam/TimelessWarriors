using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamWaveFight : TeamFight
{
        List<int> currentLife;
        List<Text> lifeText;
        public int numberEnemies = 5;
        public int enemiesdead = 0;
        GameObject enemy;
    override protected void init()
        {
            base.init();
            initLifes();
            spawnWave();
        }
    void Update()
    {
        if (enemiesdead >= numberEnemies)
            finishFight();
    }
    public override void initTeams()
    {
        team1 = new Entity[2];
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
                            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.blackKnightMaterials[1];
                        }
                        break;
                }
                team1[i] = players[i];
            }
        }

        Physics2D.IgnoreCollision(players[0].transform.GetChild(2).GetComponent<Collider2D>(), players[1].transform.GetChild(3).GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(players[1].transform.GetChild(2).GetComponent<Collider2D>(), players[0].transform.GetChild(3).GetComponent<Collider2D>());
    }
    private void initLifes()
        {
            lifeText = new List<Text>();
            currentLife = new List<int>();
            int maxLife = Settings.Instance.fightSettings.winFactorQ;
            enemy = Resources.Load("Characters/" + "Alien") as GameObject;
            Transform playerBars = GameObject.Find("PlayerBars").transform;
            for (int i = 0; i < players.Count; i++)
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
            }else if(ent is AlienAI)
        {
            ent.life.CurrentVal = 0;
        }
        }

    public override void onEntityHit(Entity hitter, Entity receiver, int damage)
    {
        if (hitter.name != "Alien(Clone)")
        {
            receiver.hittedBy = ((Character)hitter).player;
        }
        if (hitter is Character)
        {
            int hitterplayer = ((Character)hitter).player;
            statistics[(int)Utilities.statistic.DAMAGEDONE][hitterplayer] += damage;
        }
        if (hitter is Character && receiver.getLife() <= 0)
        {
            killAlien(receiver);
        }
        if (receiver is Character)
        {
            int receiverplayer = ((Character)receiver).player;
            statistics[(int)Utilities.statistic.DAMAGERECEIVED][receiverplayer] += damage;
        }
    }

    protected override void killCharacter(Character ent)
        {
            base.killCharacter(ent);
            int currentPlayer = ent.player;
            currentLife[currentPlayer]--;
            lifeText[currentPlayer].text = currentLife[currentPlayer].ToString();

            if (currentLife[currentPlayer] <= 0) finishPlayer(currentPlayer);
            else StartCoroutine(respawnCharacter((Character)ent, currentPlayer));
        }
        private void spawnWave()
        {
            Transform spawn = GameObject.Find("SpawnPositions").transform;
                startDelay();
        }
        public void startDelay()
        {
            StartCoroutine(delay());
        }
        IEnumerator delay()
        {
            for (int i = 0; i < numberEnemies; i++)
            {
                yield return new WaitForSeconds(6f);
                Instantiate(enemy, spawnPositions.GetChild(Random.Range(1, 4)).transform.position, Quaternion.identity);
            }
    }
}
