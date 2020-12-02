
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class MenuManager : MonoBehaviour{

    public static MenuManager instance { get; private set; }
    public enum mode {NONE, BRAWL, HISTORY, MINIGAMES, TOURNAMENT, TRAINING};
    public mode wichMode;
    public enum InputType {MOUSE, CONTROLLER}
    public InputType input;
    private GameObject fade; //GO that contains de fade image.
    TweenCallback call;
    public  bool isFaded;//if the fade image alpha is 1, isFaded, else is false.
    public int lastNplayersLength, currentCharacters, currentPlayers;
    public Image fade_img;
    public Sprite[] allreadyImages;
    public  List<Buttons> buttons;
    public List<MenuPlayerController> players;
    public  Dictionary<string, GameObject> panels;
    public string lastPanel;
    public  GameObject currentPanel, infoB1, infoB2, AllreadyImage;
    public Transform[] selectedPositions;
    public int numCPU;
    public List<int> Colors = new List<int>();
    public List<string> CPUs = new List<string>();

    void Awake()
    {
        call += checkCurrentPanel;
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }else
        {
            instance = this;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        panels = new Dictionary<string, GameObject>();
        buttons = new List<Buttons>();
        players = new List<MenuPlayerController>();
        SceneManager.sceneLoaded += this.OnSceneLoad;
    }
    void Start()
    {
        Utilities.fadeIn(fade_img, 0f, 1f);
        isFaded = false;
    }

    void Update()
    {

        if (InputControl.m_State == InputControl.eInputState.MouseKeyboard)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenu": updateMainMenu(); break;
            case "CharacterSelection": updateCharacterSelection(); break;
        }
    }

    private void updateCharacterSelection()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPanel = null;
            SceneManager.LoadScene("FightScene");
        }
        checkForNewController();
    }

    void checkForNewController()
    {
        //if (nplayers != lastNplayersLength)
        //{
        //    string[] nPlayersName = Input.GetJoystickNames();
        //    for (int i = 0; i < lastNplayersLength; i++)
        //    {
        //        if (nPlayersName[i] != "")
        //        {
        //            if (nplayers < lastNplayersLength) nplayers++;
        //            else nplayers--;
        //        }
        //    }
        //    for (int i = 0; i < nplayers; i++)
        //    {
        //        if (i != 0) players[i].enabled = true;         
        //    }
        //}
        //lastNplayersLength = Input.GetJoystickNames().Length;
    }

    void initButtons()
    {
        buttons.Clear();
        GameObject[] buttonsGO = GameObject.FindGameObjectsWithTag("Button");
        foreach(GameObject b in buttonsGO)
        {
            buttons.Add(b.GetComponent<Buttons>());
        }
    }

    void initPanels()
    {
        panels.Clear();
        switch(SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                panels.Add("MainMenuPanel", GameObject.Find("MainMenuPanel"));
                panels.Add("LocalMenuPanel", GameObject.Find("LocalMenuPanel"));
                panels.Add("OnlineMenuPanel", GameObject.Find("OnlineMenuPanel"));
                panels.Add("OptionsMenuPanel", GameObject.Find("OptionsMenuPanel"));
                panels.Add("SoundMenuPanel", GameObject.Find("SoundMenuPanel"));
                panels.Add("ControlMenuPanel", GameObject.Find("ControlMenuPanel"));
                infoB1 = GameObject.Find("InfoB1");
                infoB2 = GameObject.Find("InfoB2");
                panels.Add("VideoMenuPanel", GameObject.Find("VideoMenuPanel"));
                if (lastPanel != "")
                {
                    if (lastPanel == "CharacterPanel")activePanel("LocalMenuPanel");
                }
                else activePanel("MainMenuPanel");
                 break;
            case "CharacterSelection":
                panels.Add("CharacterPanel", GameObject.Find("CharacterPanel"));
                panels.Add("MapSelectionPanel", GameObject.Find("MapSelectionPanel"));
                panels.Add("MiniGameSelectionPanel", GameObject.Find("MiniGameSelectionPanel"));
                panels.Add("FightSettingsMenuPanel", GameObject.Find("FightSettingsMenuPanel"));
                panels.Add("ItemsMenuPanel", GameObject.Find("ItemsMenuPanel"));
                infoB1 = GameObject.Find("InfoB1");
                infoB2 = GameObject.Find("InfoB2");
                activePanel("CharacterPanel"); break;
            case "FightScene":
                panels.Add("PauseMenuPanel", GameObject.Find("PauseMenuPanel"));
                panels.Add("OptionsMenuPanel", GameObject.Find("OptionsMenuPanel"));
                panels.Add("SoundMenuPanel", GameObject.Find("SoundMenuPanel"));
                panels.Add("VideoMenuPanel", GameObject.Find("VideoMenuPanel"));
                //panels.Add("ControlMenuPanel", GameObject.Find("ControlMenuPanel"));
                break;
            default: break;

        }
        disableAllPanels();
    }


    void checkCurrentPanel()
    {
        foreach (GameObject p in panels.Values)
        {
            if (!p.Equals(currentPanel))
            {
                disablePanel(p);
            }
            else
            {
                enablePanelC(p);
            }
        }
       
    }

    public  void setButtonsInMainPositionAnim()
    {
        List<MainMenuButtons> mainmenubuttons = new List<MainMenuButtons>();
        foreach (Buttons b in buttons)
        {
            if (b is MainMenuButtons)
            {
                mainmenubuttons.Add((MainMenuButtons)b);
            }
        }

        foreach(MainMenuButtons mmb in mainmenubuttons)
        {
            mmb.GetComponent<RectTransform>().DOLocalMove(mmb.mainPosition, 0.5f);
        }
    }

    public  void setButtonsInEnterPosition()
    {
        List<MainMenuButtons> mainmenubuttons = new List<MainMenuButtons>();
        foreach (Buttons b in buttons)
        {
            if (b is MainMenuButtons)
            {
                mainmenubuttons.Add((MainMenuButtons)b);
            }
        }

        foreach (MainMenuButtons mmb in mainmenubuttons)
        {
            mmb.GetComponent<RectTransform>().transform.localPosition = mmb.enterPosition;
        }
    }

    public  void doChangePanelsAnim()
    {
        List<MainMenuButtons> mainmenubuttons = new List<MainMenuButtons>();
        List<WarriorSelectionButtons> warriorbuttons = new List<WarriorSelectionButtons>();
        List<MapSelectionButton> mapselectionbuttons = new List<MapSelectionButton>();
        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                foreach (Buttons b in buttons)
                {
                    if (b is MainMenuButtons)
                    {
                        mainmenubuttons.Add((MainMenuButtons)b);
                    }
                }

                foreach (MainMenuButtons mmb in mainmenubuttons)
                {
                    mmb.initPanelChangeSequence();
                }
                infoB1.GetComponent<InfoImagesController>().initPanelChangeSequence();
                infoB2.GetComponent<InfoImagesController>().initPanelChangeSequence();
                break;
            case "CharacterSelection":
                foreach (Buttons b in buttons)
                {
                    if (b is WarriorSelectionButtons)
                    {
                        warriorbuttons.Add((WarriorSelectionButtons)b);
                    }
                }

                foreach (WarriorSelectionButtons wb in warriorbuttons)
                {
                    wb.initPanelChangeSequence();
                }
                infoB1.GetComponent<InfoImagesController>().initPanelChangeSequence();
                infoB2.GetComponent<InfoImagesController>().initPanelChangeSequence();

                foreach (Buttons b in buttons)
                {
                    if (b is MapSelectionButton)
                    {
                        mapselectionbuttons.Add((MapSelectionButton)b);
                    }
                }

                foreach (MapSelectionButton mb in mapselectionbuttons)
                {
                    mb.initPanelChangeSequence();
                }
                break;
        }       

    }

    public void activePanel(string panel)
    {
        currentPanel = panels[panel];
        doChangePanelsAnim();
        DOVirtual.DelayedCall(0.25f, call, true);
        selectDefaultButton();
    }

    void disableAllPanels()
    {
        foreach(GameObject p in panels.Values)
        {
            if (!p.Equals(currentPanel))
            {
                disablePanel(p);
            }
        }
    }

    public void disablePanel(GameObject p)
    {
        switch(p.name)
        {
            case "OptionsMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                }
                break;
            case "SoundMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(0).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

                    if (p.transform.GetChild(i).GetComponent<Text>()) p.transform.GetChild(i).GetComponent<Text>().enabled = false;
                    for (int j = 0; j < p.transform.GetChild(i).transform.childCount; j++)
                    {
                        p.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
                    }
                }
                break;
            case "VideoMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {   
                    switch(i)
                    {
                        case 0: p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                            p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                            break;
                        default:
                            p.transform.GetChild(i).GetComponent<Text>().enabled = false;
                            p.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(false);
                            break;
                    }                     
                }
                break;
            case "ControlMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                }
                break;
            case "CharacterPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            case "MapSelectionPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            case "MiniGameSelectionPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            case "PauseMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                }
                break;
            case "FightSettingsMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                    switch(i)
                    {
                        case 1:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; break;
                        case 2:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; break;
                        case 3:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; break;
                    }
                }
                break;
            case "ItemsMenuPanel":
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = false;
                    if (p.transform.GetChild(i).GetComponentInChildren<Image>()) p.transform.GetChild(i).GetComponentInChildren<Image>().enabled = false;
                    switch (i)
                    {
                        case 1:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; break;
                        case 2:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; break;
                        case 3:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = false;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = false; ; break;
                    }
                }
                break;
            default:
                p.GetComponent<Image>().enabled = false;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = false;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
        }
        
    }

    public void checkAllPlayersHaveSelectedChar()
    {
        foreach(MenuPlayerController mpc in players)
        {
            if(mpc.enabled)
            {
                if(mpc.hasSelectedCharacter)
                {
                    switch (wichMode)
                    {
                        case mode.BRAWL:
                            activePanel("MapSelectionPanel");
                            switch (mpc.idPlayer)
                            {
                                case 0: mpc.playerIdentifier.transform.SetParent(mpc.selectedButton.transform);  mpc.identifierPosition = new Vector2(-121, 68); break;
                                default: mpc.playerIdentifier.SetActive(false);
                                         mpc.selectedButton = null; break;
                            }
                            mpc.playerIdentifier.transform.localPosition = mpc.identifierPosition;
                            disableAllreadyWithStyle();
                            break;
                        case mode.TRAINING:
                            activePanel("MapSelectionPanel");
                            switch (mpc.idPlayer)
                            {
                                case 0: mpc.playerIdentifier.transform.SetParent(mpc.selectedButton.transform); mpc.identifierPosition = new Vector2(-121, 68); break;
                                default:
                                    mpc.playerIdentifier.SetActive(false);
                                    mpc.selectedButton = null; break;
                            }
                            mpc.playerIdentifier.transform.localPosition = mpc.identifierPosition;
                            disableAllreadyWithStyle();
                            break;
                        case mode.HISTORY:
                            Settings.Instance.fightSettings.selectedCharacters.Add(mpc.characterSelected);
                            Settings.Instance.fightSettings.selectedSkins.Add(mpc.ColorPanel.GetComponent<ColorPanelController>().currentIdColor);
                            disableAllreadyWithStyle();
                            break;
                        case mode.MINIGAMES:
                            activePanel("MiniGameSelectionPanel");
                            switch (mpc.idPlayer)
                            {
                                case 0: mpc.playerIdentifier.transform.SetParent(mpc.selectedButton.transform); mpc.identifierPosition = new Vector2(-121, 68); break;
                                default:
                                    mpc.playerIdentifier.SetActive(false);
                                    mpc.selectedButton = null; break;
                            }
                            mpc.playerIdentifier.transform.localPosition = mpc.identifierPosition;
                            disableAllreadyWithStyle();
                            break;
                    }             
                }
            }
        }
        if(wichMode == mode.HISTORY)
        {
            if (GameManager.Instance.arcadeInstance == null)
            {
                GameObject x = Instantiate(Resources.Load("ArcadeManager"), new Vector2(0, 0), Quaternion.identity) as GameObject;
                GameManager.Instance.arcadeInstance = x.GetComponent<ArcadeController>();
            }
            
        }
    }

    public void checkAllPlayersHaveSelectedMap()
    {
        bool startGame = false;
        Settings.Instance.fightSettings.selectedCharacters.Clear();
        foreach (MenuPlayerController mpc in players)
        {
            if (mpc.hasSelectedCharacter)
            {
                if (mpc.idPlayer == 0)
                {
                    if(mpc.hasSelectedMap)
                    {
                        startGame = true;
                    }                                             
                }
            }
        }
        if (startGame)
        {
            Settings.Instance.fightSettings.mapSelection = players[0].mapSelected;
            foreach (MenuPlayerController mpc in players)
            {
                if(mpc.hasSelectedColor)
                {
                    Settings.Instance.fightSettings.selectedCharacters.Add(mpc.characterSelected);
                    Settings.Instance.fightSettings.selectedSkins.Add(mpc.ColorPanel.GetComponent<ColorPanelController>().currentIdColor);
                }               
            }

            for (int i = 0; i < numCPU; i++)
            {

                Settings.Instance.fightSettings.selectedCharacters.Add(CPUs[i]);
                Settings.Instance.fightSettings.selectedSkins.Add(UnityEngine.Random.Range(0, 4));
            }

            SceneManager.LoadScene("FightScene");
        }
    }
    public void checkAllPlayersHaveSelectedMinigame()
    {
        bool startGame = false;
        Settings.Instance.fightSettings.selectedCharacters.Clear();
        foreach (MenuPlayerController mpc in players)
        {
            if (mpc.hasSelectedCharacter)
            {
                if (mpc.idPlayer == 0)
                {
                    if (mpc.hasSelectedMinigame)
                    {
                        startGame = true;
                    }
                }
            }
        }
        if (startGame)
        {
            Settings.Instance.fightSettings.mapSelection = "BaseMap";
            Settings.Instance.fightSettings.type = players[0].minigameSelected;
            foreach (MenuPlayerController mpc in players)
            {
                if (mpc.hasSelectedColor)
                {
                    Settings.Instance.fightSettings.selectedCharacters.Add(mpc.characterSelected);
                    Settings.Instance.fightSettings.selectedSkins.Add(mpc.ColorPanel.GetComponent<ColorPanelController>().currentIdColor);
                }
            }

            for (int i = 0; i < numCPU; i++)
            {

                Settings.Instance.fightSettings.selectedCharacters.Add(CPUs[i]);
                Settings.Instance.fightSettings.selectedSkins.Add(UnityEngine.Random.Range(0, 4));
            }
            if (Settings.Instance.fightSettings.type == "TeamBossFight" || Settings.Instance.fightSettings.type == "BossFight")
            {
                Settings.Instance.fightSettings.selectedCharacters.Add("Templar_Boss");
                Settings.Instance.fightSettings.selectedSkins.Add(0);
            }
            SceneManager.LoadScene("FightScene");
        }
    }
    void enablePanelC(GameObject p)
    {
        switch (p.name)
        {
            case "OptionsMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                }
                break;
            case "SoundMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(0).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

                    switch(i)
                    {
                        case 0: break;
                        default:
                            switch (p.transform.GetChild(i).GetChild(0).name)
                            {
                                case "masterSlider":
                                    p.transform.GetChild(i).GetChild(0).GetComponent<Slider>().value = Settings.Instance.audioSettings.masterv;break;
                                case "musicSlider": p.transform.GetChild(i).GetChild(0).GetComponent<Slider>().value = Settings.Instance.audioSettings.musicv; break;
                                case "sfxSlider": p.transform.GetChild(i).GetChild(0).GetComponent<Slider>().value = Settings.Instance.audioSettings.sfxv; break;
                                case "voicesSlider": p.transform.GetChild(i).GetChild(0).GetComponent<Slider>().value = Settings.Instance.audioSettings.voicesv; break;
                            }break;
                    }

                    if (p.transform.GetChild(i).GetComponent<Text>()) p.transform.GetChild(i).GetComponent<Text>().enabled = true;
                    for (int j = 0; j < p.transform.GetChild(i).transform.childCount; j++)
                    {
                        p.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(true);
                    }
                }
                break;
            case "CharacterPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                break;

            case "MapSelectionPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                break;
            case "MiniGameSelectionPanel":
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
                break;
            case "PauseMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                }
                break;
            case "FightSettingsMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                    switch (i)
                    {
                        case 1:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                        case 2:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                        case 3:
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Image>()) p.transform.GetChild(i).transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                    }
                }
                break;
            case "ItemsMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = false;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;

                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                    if (p.transform.GetChild(i).GetComponentInChildren<Image>()) p.transform.GetChild(i).GetComponentInChildren<Image>().enabled = true;
                    switch (i)
                    {
                        case 1:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                        case 2:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                        case 3:
                            if (p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>()) p.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>().enabled = true;
                            if (p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>()) p.transform.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>().enabled = true; break;
                    }
                }
                break;
            case "VideoMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    switch (i)
                    {
                        case 0:
                            p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                            p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                            break;
                        default:
                            p.transform.GetChild(i).GetComponent<Text>().enabled = true;
                            p.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
                            break;
                    }
                }
                break;
            default:
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<PolygonCollider2D>()) p.transform.GetChild(i).GetComponent<PolygonCollider2D>().enabled = true;
                    else if (p.transform.GetChild(i).GetComponent<BoxCollider2D>()) p.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                    if (p.transform.GetChild(i).GetComponent<Text>()) p.transform.GetChild(i).GetComponent<Text>().enabled = true;
                    for (int j = 0; j < p.transform.GetChild(i).transform.childCount; j++)
                    {
                        p.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(true);
                        for (int f = 0; f < p.transform.GetChild(i).transform.childCount; f++)
                        {
                            p.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }

                break;
            case "ControlMenuPanel":
                p.GetComponent<Image>().enabled = true;
                for (int i = 0; i < p.transform.childCount; i++)
                {
                    p.transform.GetChild(i).GetComponent<Image>().enabled = true;
                    if (p.transform.GetChild(i).GetComponentInChildren<Text>()) p.transform.GetChild(i).GetComponentInChildren<Text>().enabled = true;
                }
                break;
        }
        if (p.transform.GetChild(0).GetComponent<Buttons>()) p.transform.GetChild(0).GetComponent<Buttons>().buttonHighlight();
        else if (p.transform.GetChild(1).GetComponent<Buttons>()) { p.transform.GetChild(1).GetComponent<Buttons>().buttonHighlight();
        }
        else players[0].selectedButton = null;

    }


    public void returnPanel()
    {
        switch (currentPanel.name)
        {
            case "MainMenuPanel":
                break;
            case "LocalMenuPanel":
                activePanel("MainMenuPanel");
                break;
            case "OnlineMenuPanel":
                activePanel("MainMenuPanel");
                break;
            case "OptionsMenuPanel":
                {
                    if(SceneManager.GetActiveScene().name == "MainMenu") activePanel("MainMenuPanel");
                    else activePanel("PauseMenuPanel");
                }break;
            case "SoundMenuPanel":
                Settings.Instance.SaveSettings("audio");
                activePanel("OptionsMenuPanel");
                break;
            case "VideoMenuPanel":
                activePanel("OptionsMenuPanel");
                Settings.Instance.SaveSettings("video");
                break;
            case "ControlMenuPanel":
                activePanel("OptionsMenuPanel");
                break;
            case "ItemsMenuPanel":
                activePanel("FightSettingsMenuPanel");
                break;
            case "FightSettingsMenuPanel":
                activePanel("CharacterPanel");
                break;
            case "MapSelectionPanel":
                activePanel("CharacterPanel");
                break;
            case "MiniGameSelectionPanel":
                activePanel("CharacterPanel");
                break;
            case "CharacterPanel":
                StartCoroutine(loadSceneWithFade("MainMenu"));
                break;
        }
    }

    public void initPlayers()
    {
        players.Clear();
        MenuPlayerController[] menupc = gameObject.GetComponents<MenuPlayerController>();
        foreach(MenuPlayerController m in menupc)
        {
            Destroy(m);
        }
        for (int i = 0; i <= 3; i++)
        {
            MenuPlayerController p = gameObject.AddComponent<MenuPlayerController>();
            players.Add(p);
            p.idPlayer = i;
            switch(p.idPlayer)
            {
                case 0: p.identifierPosition = new Vector2(-44, 68);
                    p.joined = true;  break;
                case 1: p.identifierPosition = new Vector2(64, 68); break;
                case 2: p.identifierPosition = new Vector2(-84, -80); break;
                case 3: p.identifierPosition = new Vector2(43, -80); break;
            }
        }

        foreach(MenuPlayerController mpc in players)
        {
            if (mpc.idPlayer != 0)
            {
                mpc.enabled = false;
            }
            else mpc.selectedButton = currentPanel.transform.GetChild(0).GetComponent<Buttons>();
        }
    }

    void updateMainMenu()
    {

    }

    void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {
        switch(SceneManager.GetActiveScene().name)
        {
            case "CharacterSelection": initCharacterSelectionScene();  break;
            case "MainMenu": initMainMenuScene(); break;
            case "FightScene": initPanels(); initButtons(); break;
        }
    }

    private void initMainMenuScene()
    {
        fade_img = GameObject.Find("fade").GetComponent<Image>();
        Utilities.fadeIn(fade_img, 0f, 1f);
        currentCharacters = 0;
        isFaded = false;
        initButtons();
        initPanels();
        initPlayers();
    }

    public void initCharacterSelectionScene()
    {
        currentPlayers = 0;
        Settings.Instance.fightSettings.selectedCharacters.Clear();
        CPUs.Clear();
        Settings.Instance.fightSettings.selectedSkins.Clear();
        Colors.Clear();
        int nplayers = Settings.Instance.fightSettings.nPlayersCanJoin;
        fade_img = GameObject.Find("fade").GetComponent<Image>();
        Utilities.fadeIn(fade_img, 0f, 1f);
        AllreadyImage = GameObject.Find("AllReady"); 
        isFaded = false;
        initButtons();
        initPanels();
        initSelectionPositions();
        numCPU = 0;
        currentCharacters = 0;
        currentPlayers = 0;
        string[] nPlayersName = Input.GetJoystickNames();
        //for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        //{
        //    if (nPlayersName[i] != "")
        //    {
        //        nplayers++;
        //    }
        //}
        for (int i = 1; i < nplayers; i++)
        {
            players[i].enabled = true;
            //players[i].selectedButton = currentPanel.transform.GetChild(0).GetComponent<Buttons>();
            //players[i].playerIdentifier = Instantiate(Resources.Load("Identifier/IdentifierP"), players[i].selectedButton.gameObject.transform) as GameObject;
            //Sprite spr = Resources.Load<Sprite>("Identifier/PS4_Circle1");
            //players[i].playerIdentifier.GetComponent<Image>().sprite = spr;
            //players[i].playerIdentifier.GetComponent<RectTransform>().localPosition = players[i].identifierPosition;
        }
        players[0].initSelectionPlayer();
        initColorPanels();
    }

    void selectDefaultButton()
    {
        foreach (MenuPlayerController mpc in players)
        {
            if (mpc.enabled)
            {
                if (currentPanel.transform.GetChild(0).GetComponent<Buttons>())
                {
                    mpc.selectedButton = currentPanel.transform.GetChild(0).GetComponent<Buttons>();
                }
                else { mpc.selectedButton = currentPanel.transform.GetChild(1).GetComponent<Buttons>(); }
            }
        }
    }

    public IEnumerator loadSceneWithFade(string scene)
    {
        Utilities.fadeIn(fade_img, 1, 0.5f);
        while (fade_img.color.a != 1)
        {
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }

    void initSelectionPositions()
    {
        Transform selectedPos = GameObject.Find("SelectedPositions").transform;
        int count = selectedPos.childCount;
        selectedPositions = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            selectedPositions[i] = selectedPos.transform.GetChild(i);
        }
        switch(wichMode)
            {
            case mode.BRAWL: break;
            case mode.TRAINING: break;
            case mode.HISTORY:
                for (int i = 2; i < count; i++)
                {
                    selectedPositions[i].gameObject.SetActive(false);
                }

                GameObject.Find("FightSettings").SetActive(false);
                panels["CharacterPanel"].transform.GetChild(0).GetComponent<WarriorSelectionButtons>().left = panels["CharacterPanel"].transform.GetChild(3).GetComponent<WarriorSelectionButtons>();
                panels["CharacterPanel"].transform.GetChild(4).GetComponent<WarriorSelectionButtons>().left = panels["CharacterPanel"].transform.GetChild(7).GetComponent<WarriorSelectionButtons>();
                break;
            case mode.MINIGAMES:
                for (int i = 2; i < count; i++)
                {
                    selectedPositions[i].gameObject.SetActive(false);
                }

                GameObject.Find("FightSettings").SetActive(false);
                panels["CharacterPanel"].transform.GetChild(0).GetComponent<WarriorSelectionButtons>().left = panels["CharacterPanel"].transform.GetChild(3).GetComponent<WarriorSelectionButtons>();
                panels["CharacterPanel"].transform.GetChild(4).GetComponent<WarriorSelectionButtons>().left = panels["CharacterPanel"].transform.GetChild(7).GetComponent<WarriorSelectionButtons>();
                break;
        }
        
    }

    void initColorPanels()
    {
        foreach(MenuPlayerController mpc in players)
        {
            if (mpc.enabled)
            {
                mpc.ColorPanel = GameObject.Find("ColorSelection" + (mpc.idPlayer + 1));
                mpc.ColorPanel.GetComponent<ColorPanelController>().disable();
            }
        }
    }
    public bool hasAllPlayerSelected()
    {
        int i = 0;
        foreach(MenuPlayerController mpc in players)
        {
            if(mpc.joined)
            {
                if(mpc.hasSelectedCharacter)
                {
                    i++;
                }
            }
        }

        if (i >= currentCharacters - numCPU)
        {
            return true;
        }
        else return false;
    }

    public bool hasAllPlayerSelectedColor()
    {
        int i = 0;
        foreach (MenuPlayerController mpc in players)
        {
            if (mpc.joined)
            {
                if (mpc.hasSelectedColor)
                {
                    i++;
                }
            }
        }

        if (i >= currentCharacters - numCPU)
        {
            return true;
        }
        else return false;
    }

    public bool hasAllPlayerSelectedMap()
    {
        if (players[0].hasSelectedMap) return true;
        else return false;
    }
    public bool hasAllPlayerSelectedMinigame()
    {
        if (players[0].hasSelectedMinigame) return true;
        else return false;
    }

    public void disableAllreadyWithStyle()
    {
        AllreadyImage.GetComponent<RectTransform>().DOScaleY(0f, 0f);
    }

    public void enableAllreadyWithStyle()
    {
        AllreadyImage.GetComponent<RectTransform>().DOScaleY(1, 0.05f);
    }

}
