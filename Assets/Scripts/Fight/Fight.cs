using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/// <summary>
/// Base Class for every type of Fight.
/// </summary>
public class Fight : MonoBehaviour {

    public int totalStatistics = 10;
    public GameObject countdownGO;
    public List<Entity> players;
    protected Transform spawnPositions, characterBars;
    protected RectTransform finishImage;
    public List<List<int>> statistics;
    public ItemSpawner itemSpawner;
    public CameraControl camC;
    public bool isPaused, canPause;
    public ArrowScript[] targets;
    public AudioSource[] audioS;
    public List<string> statisticNames = new List<string>{ "DAMAGEDONE", "DAMAGERECEIVED", "KOS", "PERFECTS", "KILLSPREE", "DEATHS", "FALLS", "ULTIMATES" };
    int finishedPlayers;

    /// <summary>
    /// Initialize components of Fight.
    /// </summary>
    virtual protected void init() {
        DontDestroyOnLoad(this);
        finishImage = GameObject.Find("FinishImage").GetComponent<RectTransform>();
        countdownGO = GameObject.Find("Countdown");
        itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
        finishImage.gameObject.SetActive(false);
        camC = Camera.main.GetComponent<CameraControl>();
        targets = camC.transform.GetComponentsInChildren<ArrowScript>();
        spawnCharacters();
        initStatistics();
        audioS = GetComponents<AudioSource>();
    }
    /// <summary>
    /// Initialize statistics for each player in game.
    /// </summary>
    public void initStatistics() {
        statistics = new List<List<int>>();
        for (int i = 0; i < totalStatistics; i++)
        {
            statistics.Add(new List<int>());
            for (int x = 0; x < players.Count; x++)
            {
                statistics[i].Add(0);
            }            
        }
    }
    /// <summary>
    /// Called when an Entity hits the DeathZone.
    /// </summary>
    /// <param name="ent"></param>
    virtual public void onBorderCollision(Entity ent) {
        audioS[2].clip = SoundManager.instance.audioclips["deathSFX"];
        audioS[2].Play();
        if (ent.speciality is BlackTemplar) ((BlackTemplar)ent.speciality).resetShadow();
    }
    /// <summary>
    /// Called when an Entity is hitted by an Attack.
    /// </summary>
    /// <param name="hitter"></param>
    /// <param name="receiver"></param>
    /// <param name="damage"></param>
    virtual public void onEntityHit(Entity hitter, Entity receiver, int damage) {
        int hitterplayer = ((Character)hitter).player;
        int receiverplayer = ((Character)receiver).player;

        if (hitter is Character) statistics[(int)Utilities.statistic.DAMAGEDONE][hitterplayer] += damage;
        if (receiver is Character)
        {
            statistics[(int)Utilities.statistic.DAMAGERECEIVED][receiverplayer] += damage;
            if (hitter is Character)
            {
                receiver.hittedBy = hitterplayer;
                StopCoroutine("resetHittedBy");
                StartCoroutine("resetHittedBy", hitterplayer);
            }
        }

        audioS[0].clip = SoundManager.instance.audioclips["hitSFX"];
        audioS[0].Play();
    }
    virtual public void onPickupItem(Item it) { }

    /// <summary>
    /// Spawns the selected Characters for the Fight.
    /// </summary>
    protected void spawnCharacters()
    {
        spawnPositions = GameObject.Find("SpawnPositions").transform;
        characterBars = GameObject.Find("PlayerBars").transform;
        players = new List<Entity>();

        BarScript[] bars = characterBars.GetComponentsInChildren<BarScript>();
        
        int totalPlayers = Settings.Instance.fightSettings.selectedCharacters.Count;
        int i;
        for (i = 0; i < totalPlayers; i++)
        {
            string characterSelection = Settings.Instance.fightSettings.selectedCharacters[i];
            string[] split = characterSelection.Split('_');
            int selectedSkin = Settings.Instance.fightSettings.selectedSkins[i];
            GameObject instance = Instantiate(GameManager.Instance.characterPrefabs[characterSelection], spawnPositions.GetChild(i).transform.position, Quaternion.identity) as GameObject;
            Camera cam = instance.transform.GetChild(4).GetComponent<Camera>();
            cam.backgroundColor = Settings.Instance.fightSettings.colors[i];
            cam.targetTexture = GameManager.Instance.textures[i];
            targets[i].GetComponent<SpriteRenderer>().color = Settings.Instance.fightSettings.colors[i];
            targets[i].target = instance;
            targets[i].enabled = true;
            Character cht = instance.GetComponent<Character>();

            characterBars.GetChild(i).gameObject.SetActive(true);
            characterBars.GetChild(i).transform.GetChild(2).GetComponent<Image>().color = Settings.Instance.fightSettings.colors[i];
            characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().sprite = GameManager.Instance.spritesUI["HUDImage"+split[0]];

            
            GameObject identifier = new GameObject("Identifier");
            identifier.transform.position = new Vector3(0,2.2f,0);
            identifier.transform.SetParent(cht.transform, false);
            SpriteRenderer spr = identifier.AddComponent<SpriteRenderer>();


            if(split[1] == "AI") spr.sprite = GameManager.Instance.spritesUI["IdentifierCPU"];
            else
            {
                spr.sprite = GameManager.Instance.spritesUI["IdentifierP" + (i + 1)];
                spr.color = Settings.Instance.fightSettings.colors[i];
            }
            spr.sortingOrder = 5;
            cht.identifier = spr;

            //Setup materials of selected skin
            switch (split[0])
            {
                case "Templar":
                    {
                        instance.GetComponent<SpriteRenderer>().material = GameManager.Instance.templarMaterials[selectedSkin];
                        characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.templarMaterials[selectedSkin];
                    }
                    break;
                case "Valkyrie":
                    {
                        instance.GetComponent<SpriteRenderer>().material = GameManager.Instance.valkyrieMaterials[selectedSkin];
                        characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[selectedSkin];
                    }
                    break;
                case "Pirate":
                    {
                        instance.GetComponent<SpriteRenderer>().material = GameManager.Instance.pirateMaterials[selectedSkin];
                        characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[selectedSkin];
                    }
                    break;
                case "BlackTemplar":
                    {
                        instance.GetComponent<SpriteRenderer>().material = GameManager.Instance.blackKnightMaterials[selectedSkin];
                        characterBars.GetChild(i).transform.GetChild(3).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[selectedSkin];
                    }
                    break;
            }

            //setup character components
            cht.speciality.instantiateSpecialityUI(characterBars.GetChild(i));
            cht.player = i;
            cht.Life.Bar = bars[2*i];
            cht.Energy.Bar = bars[1+(2*i)];
            cht.Life.Initialize();
            cht.Energy.Initialize();
            if (i % 2 != 0)
            {
                cht.facingDirection = Utilities.direction.Left;
                cht.transform.localScale = new Vector3(-cht.transform.localScale.x, cht.transform.localScale.y, 1);
                spr.flipX = true;
            }
            cht.enabled = false;

            players.Add(cht);
            camC.playersEntity.Add(cht);
        }

        //deactivate characters for countdown.
        for(int x = i;  x < 4; x++)
        {
            characterBars.GetChild(x).gameObject.SetActive(false);
        }
        camC.enabled = true;
    }

    /// <summary>
    /// Called when an Ultimate is used.
    /// </summary>
    /// <param name="player"></param>
    public void onUltimateUse(int player)
    {
        statistics[(int)Utilities.statistic.ULTIMATES][player]++;
    }

    public void onPlayerOutOfCamera(int player)
    {
        targets[player].transform.GetComponent<SpriteRenderer>().enabled = true;
        targets[player].transform.parent.GetComponent<MeshRenderer>().enabled = true;
    }
    public void onPlayerInsideCamera(int player)
    {
        targets[player].transform.GetComponent<SpriteRenderer>().enabled = false;
        targets[player].transform.parent.GetComponent<MeshRenderer>().enabled = false;
    }

    /// <summary>
    /// Kills a character, seting up statistics and state.
    /// </summary>
    /// <param name="ent"></param>
    virtual protected void killCharacter(Character ent)
    {
        if (!ent.isDead)
        {
            ent.enabled = false;
            ent.isDead = true;
            ent.RigidBody.bodyType = RigidbodyType2D.Kinematic;
            ent.setLife(0);
            ent.Animator.Play("Falling");
            if(ent.state == Utilities.state.Attacking || ent.state == Utilities.state.Ultimate) ent.stopAttacking();

            int currentPlayer = ent.player;
            int lasthitter = ent.hittedBy;

            onPlayerInsideCamera(currentPlayer);
            if (lasthitter >= 0)
            {
                Entity lasthitterEntity = players[lasthitter].GetComponent<Entity>();
                if (lasthitterEntity.getLife() == lasthitterEntity.getMaxLife()) statistics[(int)Utilities.statistic.PERFECTS][lasthitter]++;
                statistics[(int)Utilities.statistic.KOS][lasthitter]++;
                statistics[(int)Utilities.statistic.CURRENTSPREE][lasthitter]++;
                statistics[(int)Utilities.statistic.DEATHS][currentPlayer]++;
            }
            else statistics[(int)Utilities.statistic.FALLS][currentPlayer]++;

            int currentSpree = statistics[(int)Utilities.statistic.CURRENTSPREE][currentPlayer];
            if (currentSpree > statistics[(int)Utilities.statistic.KILLSPREE][currentPlayer]) statistics[(int)Utilities.statistic.KILLSPREE][currentPlayer] = currentSpree;
            statistics[(int)Utilities.statistic.CURRENTSPREE][currentPlayer] = 0;


        }     
    }

    /// <summary>
    /// Resume the actual Fight.
    /// </summary>
    internal void resume()
    {
        Time.timeScale = 1;
        foreach (Entity e in players)
        {
            e.RigidBody.gravityScale = e.gravityAmount;
            e.enabled = true;
        }
        foreach (GameObject i in itemSpawner.currentItems)
        {
            Item item = i.GetComponent<Item>();
            item.getRigidBody().gravityScale = 1;
            item.enabled = true;
        }
        MenuManager.instance.disablePanel(MenuManager.instance.currentPanel);
        MenuManager.instance.currentPanel = null;
        isPaused = false;
    }

    /// <summary>
    ///Called when an Item is destroyed.
    /// </summary>
    /// <param name="i"></param>
    public void onObjectDestroyed(GameObject i)
    {
        itemSpawner.currentItems.Remove(i);
    }

    /// <summary>
    /// Pauses the Fight.
    /// </summary>
    public void pause()
    {
        if (canPause) {
            Time.timeScale = 0;
            foreach (Entity e in players)
            {
                e.RigidBody.gravityScale = 0;
                e.enabled = false;
            }
            foreach (GameObject i in itemSpawner.currentItems)
            {
                Item item = i.GetComponent<Item>();
                item.getRigidBody().gravityScale = 0;
                item.enabled = false;
            }
            isPaused = true;
        } 
    }

    /// <summary>
    /// Makes the killed character respawn after 2 seconds, setting up its components.
    /// </summary>
    /// <param name="ent"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    protected IEnumerator respawnCharacter(Character ent, int player)
    {
        yield return new WaitForSeconds(2);
        ent.enabled = true;
        ent.isDead = false;
        ent.State = Utilities.state.Idle;
        ent.RigidBody.bodyType = RigidbodyType2D.Dynamic;

        ent.setLife(ent.getMaxLife());
        ent.becomeImmune();
        ent.RigidBody.velocity = new Vector2(0, 0);
        ent.transform.position = spawnPositions.GetChild(player).position;
        ent.Invoke("becomeHittable", 3);
        audioS[1].clip = SoundManager.instance.audioclips["respawnSFX"];
        audioS[1].Play();
    }

    /// <summary>
    /// Finishes the actual Fight, setting up ranks for each player and ends killspree.
    /// </summary>
    virtual protected void finishFight()
    {
        Time.timeScale = 0.2f;
        finishImage.gameObject.SetActive(true);
        finishImage.DOScale(0.5f, 0.3f);

        for(int i=0; i<players.Count; i++)
        {
            if (statistics[(int)Utilities.statistic.RANK][i] == 0) statistics[(int)Utilities.statistic.RANK][i] = 1;
            int currentSpree = statistics[(int)Utilities.statistic.CURRENTSPREE][i];
            if (currentSpree > statistics[(int)Utilities.statistic.KILLSPREE][i]) statistics[(int)Utilities.statistic.KILLSPREE][i] = currentSpree;
        }

        Invoke("swapScene", 25f*Time.unscaledDeltaTime);
    }

    virtual protected void Awake()
    {
        init();
    }

    /// <summary>
    /// Countdown to start the Fight.
    /// </summary>
    /// <returns></returns>
    public IEnumerator countdown()
    {
        for(int i = 0; i<4; i++)
        {
            RectTransform child = countdownGO.transform.GetChild(i).GetComponent<RectTransform>();
            child.gameObject.SetActive(true);
            child.DOScale(0.35f, 1f);
            yield return new WaitForSeconds(1);
            child.gameObject.SetActive(false);
        }
        startFight();
    }

 
    virtual protected void startFight()
    {
        foreach(Entity e in players)
        {
            e.enabled = true;
        }
        canPause = true;
    }

    /// <summary>
    /// Resets last hitter of player after 4 seconds.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public IEnumerator resetHittedBy(int player)
    {
        yield return new WaitForSeconds(4);
        players[player].GetComponent<Entity>().hittedBy = -1;
    }

    /// <summary>
    /// Finishes a player , preventing him from respawning.
    /// </summary>
    /// <param name="player"></param>
    public void finishPlayer(int player)
    {
        int totalPlayers = players.Count;
        int rank = totalPlayers - finishedPlayers;

        statistics[(int)Utilities.statistic.RANK][player] = rank;
        finishedPlayers++;

        if (fightIsOver()) finishFight();
    }

    /// <summary>
    /// Checks if Fight has to end.
    /// </summary>
    /// <returns></returns>
    virtual public bool fightIsOver()
    {
        int totalPlayers = players.Count;
        if (totalPlayers - finishedPlayers <= 1) return true;
        else return false;
    }

    public void swapScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("ScoreScreen");
    }
    public void killAlien(Entity ent)
    {
        if (!ent.isDead)
        {
            //ent.enabled = false;
            ent.isDead = true;
            ent.RigidBody.gravityScale = 0;
            int lasthitter = ent.hittedBy;
            if (lasthitter >= 0)
            {
                Entity lasthitterEntity = players[lasthitter].GetComponent<Entity>();
                if (lasthitterEntity.getLife() == lasthitterEntity.getMaxLife()) statistics[(int)Utilities.statistic.PERFECTS][lasthitter]++;
                statistics[(int)Utilities.statistic.KOS][lasthitter]++;
                statistics[(int)Utilities.statistic.CURRENTSPREE][lasthitter]++;
            }
        }
    }
}
