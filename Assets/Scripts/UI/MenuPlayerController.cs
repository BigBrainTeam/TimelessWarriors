using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MenuPlayerController: MonoBehaviour {

    public Buttons selectedButton;
    public int idPlayer, locationCPU;
    public GameObject playerIdentifier;
    public Vector3 identifierPosition;
    public Sprite characterSelectedImage;
    public GameObject ColorPanel;
    TweenCallback moveOn;
    float xAxis, yAxis;
    public bool canMove, isCPU;
    public bool joined, hasSelectedCharacter, hasInited, hasSelectedMap,hasSelectedMinigame, hasSelectedColor;
    Vector2 mousePosition, lastMousePosition;
    public string characterSelected, mapSelected,minigameSelected;
    public List<string> characterSelectedIA;

    private void Awake()
    {
        moveOn += setCanMoveOn;
    }

    void Start()
    {
        canMove = true;
        hasSelectedCharacter = false;
        hasSelectedMap = false;
        hasSelectedMinigame = false;
        if (Settings.Instance.fightSettings.type != "TeamFight") hasSelectedColor = false;
        characterSelectedIA = new List<string>();
        MenuManager.instance.numCPU = 0;
    }

    void onEnable()
    {
        if (idPlayer == 0) joined = true;
        else joined = false;
    }
    void Update()
    {
        if (joined)
        {
                if (MenuManager.instance.currentPanel)
            {
                xAxis = Input.GetAxis("HorizontalP" + (idPlayer + 1));
                yAxis = Input.GetAxis("VerticalP" + (idPlayer + 1));
                mousePosition = Input.mousePosition;

                if (selectedButton != null)
                {
                    if (selectedButton.GetComponent<WarriorSelectionButtons>())
                    {
                        characterSelectedImage = selectedButton.GetComponent<WarriorSelectionButtons>().characterPose;
                    }
                }

                checkInput();
                lastMousePosition = mousePosition;
                if (SceneManager.GetActiveScene().name == "CharacterSelection")
                {
                    if (MenuManager.instance.currentPanel.name != "CharacterPanel" && MenuManager.instance.currentPanel.name != "MapSelectionPanel" && MenuManager.instance.currentPanel.name != "MinigameSelectionPanel")
                    {
                        playerIdentifier.GetComponent<Image>().enabled = false;
                    }else
                    {
                        if(playerIdentifier != null)playerIdentifier.GetComponent<Image>().enabled = true;
                    }
                }
            }
            else checkInputInGame();
        }
        else
        {
            selectedButton = null;
            if (Input.GetButtonDown("JumpP" + (idPlayer + 1)))
            {
                if (SceneManager.GetActiveScene().name == "CharacterSelection")
                {
                    if(MenuManager.instance.numCPU > 0)cleanIAinSelectPosition();
                    if (!hasInited)
                    {                      
                        initSelectionPlayer();
                    }
                    else joinPlayer();
                }
            }
        }
    }

    bool hasInputController()
    {
        if (Input.GetButtonDown("JumpP" + (1)) || Input.GetButtonDown("SpecialAttackP" + (1)) || xAxis != 0 || yAxis != 0)
        {
            return true;
        }
        else return false;
    }
    void checkInput()
    {
        if (idPlayer == 0)
        {
            if (InputControl.m_State == InputControl.eInputState.Controler && canMove)
            {
                if (MenuManager.instance.currentPanel.name == "CharacterPanel")
                {
                    if (!hasSelectedCharacter)
                    {
                        checkMovement();
                    }
                }
                else if (MenuManager.instance.currentPanel.name == "MapSelectionPanel")
                {
                    if (!hasSelectedMap)
                    {
                        checkMovement();
                    }
                }
                else if (MenuManager.instance.currentPanel.name == "MinigameSelectionPanel")
                {
                    if (!hasSelectedMinigame)
                    {
                        checkMovement();
                    }
                }else
                {
                    checkMovement();
                }
            }
        }
        else if (canMove)
        {
            if (MenuManager.instance.currentPanel.name == "CharacterPanel")
            {
                if (!hasSelectedCharacter)
                {
                    checkMovement();
                }
            }
            else if (MenuManager.instance.currentPanel.name == "MapSelectionPanel")
            {
                if (!hasSelectedMap)
                {
                    checkMovement();
                }
            }
            else if (MenuManager.instance.currentPanel.name == "MinigameSelectionPanel")
            {
                if (!hasSelectedMinigame)
                {
                    checkMovement();
                }
            }
        }
        checkActions();
        checkMouseInputs();    
    }

    void checkInputInGame()
    {
        if (Input.GetButtonDown("StartP" + (idPlayer + 1)))
        {
            if (GameManager.Instance.currentFight.canPause)
            {
                if (!GameManager.Instance.currentFight.isPaused)
                {
                    MenuManager.instance.activePanel("PauseMenuPanel");
                    GameManager.Instance.currentFight.pause();
                }
                else
                {
                    GameManager.Instance.currentFight.resume();
                }
            }          
        }
    }

    void checkMovement()
    {
        bool hasMoved = false;
        if (yAxis < 0 || Input.GetAxis("Horizontal") < 0)
        {
            setBotButton();
            hasMoved = true;
        }
        else if (yAxis > 0 || Input.GetAxis("Horizontal") > 0)
        {
            setTopButton();
            hasMoved = true;
        }

        if (xAxis < 0 || Input.GetAxis("Vertical") < 0)
        {
            setLeftButton();
            hasMoved = true;
        }
        else if (xAxis > 0 || Input.GetAxis("Vertical") > 0)
        {
            setRightButton();
            hasMoved = true;
        }

        if(MenuManager.instance.currentPanel.name == "SoundMenuPanel")
        {
            if (xAxis > 0)
            {
                switch (selectedButton.name)
                {
                    case "Master": selectedButton.gameObject.GetComponentInChildren<Slider>().value += (int)Mathf.Lerp(0, 10, xAxis); break;
                    case "Music": selectedButton.gameObject.GetComponentInChildren<Slider>().value += (int)Mathf.Lerp(0, 10, xAxis); break;
                    case "SFX": selectedButton.gameObject.GetComponentInChildren<Slider>().value += (int)Mathf.Lerp(0, 10, xAxis); break;
                    case "Voices": selectedButton.gameObject.GetComponentInChildren<Slider>().value += (int)Mathf.Lerp(0, 10, xAxis); break;
                }
            }else if(xAxis < 0)
            {
                switch (selectedButton.name)
                {
                    case "Master": selectedButton.gameObject.GetComponentInChildren<Slider>().value -= (int)Mathf.Lerp(0, 10, -xAxis); break;
                    case "Music": selectedButton.gameObject.GetComponentInChildren<Slider>().value -= (int)Mathf.Lerp(0, 10, -xAxis); break;
                    case "SFX": selectedButton.gameObject.GetComponentInChildren<Slider>().value -= (int)Mathf.Lerp(0, 10, -xAxis); break;
                    case "Voices": selectedButton.gameObject.GetComponentInChildren<Slider>().value -= (int)Mathf.Lerp(0, 10, -xAxis); break;
                }
            }
        }

        if (MenuManager.instance.currentPanel.name == "FightSettingsMenuPanel")
        {
            if (xAxis > 0)
            {
                switch (selectedButton.name)
                {
                    case "FightType": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().changeState(); break;
                    case "FinalFactor": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().increaseFactor() ; break;
                    case "StageChoice": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().changeStageChoice(); break;
                }
            }
            else if (xAxis < 0)
            {
                switch (selectedButton.name)
                {
                    case "FightType": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().changeState(); break;
                    case "FinalFactor": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().decreaseFactor(); break;
                    case "StageChoice": selectedButton.gameObject.GetComponentInParent<FightSettingsController>().changeStageChoice(); break;
                }
            }
        }

        if (hasMoved)
        {
            DOVirtual.DelayedCall(0.2f, moveOn, true);
            canMove = false;
            if(MenuManager.instance.currentPanel.name == "CharacterPanel" && selectedButton.GetComponent<WarriorSelectionButtons>())
            {
                MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().bgCharacter;
                MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().characterPose;
            }
        }
       
    }


    void checkActions()
    {
        if(Input.GetButtonDown("JumpP"+(idPlayer+1)))
        {
            if (!joined)
            {
                selectedButton = MenuManager.instance.currentPanel.transform.GetChild(0).GetComponent<Buttons>();
            }
            else
            {
                if (SceneManager.GetActiveScene().name == "CharacterSelection")
                {

                        if (MenuManager.instance.currentPanel.name == "CharacterPanel")
                        {
                        if (hasSelectedCharacter)
                        {
                            if (hasSelectedColor)
                            {
                                if (Settings.Instance.fightSettings.type != "TeamFight")
                                {
                                    if (MenuManager.instance.wichMode == MenuManager.mode.BRAWL)
                                    {
                                        if (MenuManager.instance.hasAllPlayerSelectedColor() && MenuManager.instance.currentCharacters > 1) MenuManager.instance.checkAllPlayersHaveSelectedChar();
                                    }
                                    else
                                    {
                                        if (MenuManager.instance.hasAllPlayerSelectedColor()) MenuManager.instance.checkAllPlayersHaveSelectedChar();
                                    }
                                }                             
                            }
                            else
                            {
                                if(Settings.Instance.fightSettings.type == "TeamFight")
                                {
                                    if (MenuManager.instance.currentCharacters == 4)
                                    {
                                        foreach (MenuPlayerController mpc in MenuManager.instance.players)
                                        {
                                            if (mpc.joined) mpc.hasSelectedColor = true;
                                        }
                                        if (MenuManager.instance.hasAllPlayerSelectedColor())
                                        {
                                            MenuManager.instance.checkAllPlayersHaveSelectedChar();
                                        }
                                    }
                                }
                                else
                                {
                                    if (!ColorPanel)
                                    {
                                        enableColorPanel();                          
                                    }
                                    else ColorPanel.GetComponent<ColorPanelController>().selectColor(idPlayer);
                                }
                                
                            }
                        }
                        else
                        {
                            if (selectedButton.GetComponent<WarriorSelectionButtons>())
                            {
                                if (selectedButton.GetComponent<WarriorSelectionButtons>().characterName != "null")
                                {
                                        selectCharacter();
                                    if(Settings.Instance.fightSettings.type == "TeamFight" && MenuManager.instance.currentCharacters == 4) { 
                                        MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[0];
                                        MenuManager.instance.enableAllreadyWithStyle();
                                    }
                                }
                            }
                        }
                                          
                        }
                        else if (MenuManager.instance.currentPanel.name == "MapSelectionPanel")
                        {
                            if (hasSelectedMap) MenuManager.instance.checkAllPlayersHaveSelectedMap();
                            else {
                                selectedButton.buttonHighlightOff();
                                selectedButton.onClickButton(idPlayer);
                            }
                        }
                    else if (MenuManager.instance.currentPanel.name == "MiniGameSelectionPanel")
                    {
                        if (hasSelectedMinigame) MenuManager.instance.checkAllPlayersHaveSelectedMinigame();
                        else
                        {
                            selectedButton.buttonHighlightOff();
                            selectedButton.onClickButton(idPlayer);
                        }
                    }

                    if (MenuManager.instance.currentPanel.name == "ItemsMenuPanel")
                    {
                        selectedButton.onClickButton();                   
                    }

                    if (MenuManager.instance.currentPanel.name == "FightSettingsMenuPanel")
                    {
                        if (selectedButton.gameObject.name == "Apply")
                        {                           
                            selectedButton.onClickButton();
                        }
                        else if (selectedButton.gameObject.name == "Items")
                        {
                            selectedButton.onClickButton();
                        }
                    }

                    if(selectedButton.name == "EditSettings")
                    {
                        selectedButton.onClickButton();
                    }
                }
                else
                {
                    selectedButton.buttonHighlightOff();
                    selectedButton.onClickButton();
                }
            }
        }


        if (Input.GetButtonDown("BasicAttackP" + (idPlayer + 1)))
        {
            checkCanJoinCPU();
        }

        if (Input.GetButtonDown("FinalAttackP" + (idPlayer + 1)))
        {
            checkCanUnJoinCPU();
        }

        if (Input.GetButtonDown("SpecialAttackP" + (idPlayer + 1)))
        {

            SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["backSelectButtonSFX"];
            SoundManager.instance.SFXSource.Play();
            if (SceneManager.GetActiveScene().name == "CharacterSelection")
            {
                if (MenuManager.instance.currentPanel.name == "CharacterPanel")
                {
                    if (hasSelectedCharacter)
                    {
                        if (hasSelectedColor)
                        {
                            if (Settings.Instance.fightSettings.type != "TeamFight") ColorPanel.GetComponent<ColorPanelController>().unselectColor(idPlayer);
                            else
                            {
                                unselectCharacter();
                            }
                        }
                        else
                        {
                            unselectCharacter();
                            if (Settings.Instance.fightSettings.type == "TeamFight") MenuManager.instance.disableAllreadyWithStyle();
                        }
                    }
                    else if (idPlayer == 0)
                    {
                        MenuManager.instance.selectedPositions = null;
                        playerIdentifier = null;
                        joined = false;
                        hasInited = false;
                        characterSelectedImage = null;
                        MenuManager.instance.lastPanel = MenuManager.instance.currentPanel.name;
                        StartCoroutine(MenuManager.instance.loadSceneWithFade("MainMenu"));
                    }
                    else
                    {
                        unJoinPlayer();
                        if (Settings.Instance.fightSettings.type == "TeamFight")
                        {
                            MenuManager.instance.disableAllreadyWithStyle();
                        }
                    }

                }
                else if (MenuManager.instance.currentPanel.name == "MapSelectionPanel")
                {
                    if (!hasSelectedMap)
                    {
                        MenuManager.instance.returnPanel();
                        playerIdentifier.transform.SetParent(selectedButton.transform);
                        switch (idPlayer)
                        {
                            case 0: identifierPosition = new Vector2(-44, 68); break;
                            case 1: identifierPosition = new Vector2(64, 68); break;
                            case 2: identifierPosition = new Vector2(-84, -80); break;
                            case 3: identifierPosition = new Vector2(43, -80); break;
                        }
                        playerIdentifier.transform.localPosition = identifierPosition;
                        if (Settings.Instance.fightSettings.type != "TeamFight")
                        {
                            ColorPanel.GetComponent<ColorPanelController>().unselectColor(idPlayer);
                        }
                        hasSelectedColor = false;
                        unselectCharacter();
                        MenuManager.instance.players[idPlayer].hasSelectedColor = false;
                    }
                    else
                    {
                        unselectMap();
                    }
                }
                else if (MenuManager.instance.currentPanel.name == "MinigameSelectionPanel")
                {
                    if (!hasSelectedMinigame)
                    {
                        MenuManager.instance.returnPanel();
                        playerIdentifier.transform.SetParent(selectedButton.transform);
                        switch (idPlayer)
                        {
                            case 0: identifierPosition = new Vector2(-44, 68); break;
                            case 1: identifierPosition = new Vector2(64, 68); break;
                            case 2: identifierPosition = new Vector2(-84, -80); break;
                            case 3: identifierPosition = new Vector2(43, -80); break;
                        }
                        playerIdentifier.transform.localPosition = identifierPosition;
                        hasSelectedColor = false;
                        unselectCharacter();
                        MenuManager.instance.players[idPlayer].hasSelectedColor = false;
                    }
                    else
                    {
                        unselectMap();
                    }
                }
                else if (MenuManager.instance.currentPanel.name == "ItemsMenuPanel")
                {
                    ItemSelectionButton[] items;
                    items = FindObjectsOfType<ItemSelectionButton>();
                    Settings.Instance.fightSettings.selectedItems.Clear();
                    foreach (ItemSelectionButton isb in items)
                    {
                        if (isb.on)
                        {
                            Settings.Instance.fightSettings.selectedItems.Add(isb.name);
                        }
                    }
                    MenuManager.instance.returnPanel();
                    selectedButton.buttonHighlightOff();
                }
                else if (MenuManager.instance.currentPanel.name == "FightSettingsMenuPanel")
                {
                    selectedButton.buttonHighlightOff();
                    MenuManager.instance.returnPanel();
                }
            }
            else
            {
                if (selectedButton)
                {
                    if (selectedButton.GetComponent<VideoSettingsButton>())
                    {
                        if(selectedButton.GetComponent<VideoSettingsButton>().isOnDropDown)
                        {
                            selectedButton.GetComponent<VideoSettingsButton>().isOnDropDown = false;
                            EventSystem.current.SetSelectedGameObject(null);
                        }
                        else
                        {
                            selectedButton.buttonHighlightOff();
                            MenuManager.instance.returnPanel();
                        }
                    }
                    else if(selectedButton.name == "ScreenMode")
                    {
                        selectedButton.buttonHighlight();
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                    else{
                        selectedButton.buttonHighlightOff();
                        MenuManager.instance.returnPanel();
                    }
                }
                else if (MenuManager.instance.currentPanel.name == "ControlMenuPanel")
                {
                    MenuManager.instance.returnPanel();
                }
            }
        }
        if (Input.GetButtonDown("StartP" + (idPlayer + 1)))
        {
            if (MenuManager.instance.wichMode == MenuManager.mode.BRAWL)
            {
                if (SceneManager.GetActiveScene().name == "CharacterSelection")
                {
                    if (MenuManager.instance.currentPanel.name == "CharacterPanel")
                    {
                        selectedButton.buttonHighlightOff();
                        playerIdentifier.transform.SetParent(MenuManager.instance.panels["CharacterPanel"].transform.GetChild(0).transform);
                        playerIdentifier.transform.localPosition = MenuManager.instance.players[idPlayer].identifierPosition;
                        MenuManager.instance.activePanel("FightSettingsMenuPanel");                   
                    }
                }
                else if (SceneManager.GetActiveScene().name == "FightScene") GameManager.Instance.currentFight.resume();
            }    
        }

        if (Input.GetButtonDown("RBP" + (idPlayer + 1)))
        {
            if (ColorPanel)
            {
                if(!hasSelectedColor)
                {
                    ColorPanel.GetComponent<ColorPanelController>().changeColorRight();
                }
                
            }     
        }

        if (Input.GetButtonDown("LBP" + (idPlayer + 1)))
        {
            if (ColorPanel)
            {
                if (!hasSelectedColor)
                {
                    ColorPanel.GetComponent<ColorPanelController>().changeColorLeft();
                }
            }
        }
    }

    public void checkCanUnJoinCPU()
    {
        if (SceneManager.GetActiveScene().name == "CharacterSelection")
        {
            if (MenuManager.instance.numCPU > 0) unJoinCPU();
        }
    }

    public void checkCanJoinCPU()
    {
        if (idPlayer == 0)
        {
            if (SceneManager.GetActiveScene().name == "CharacterSelection")
            {
                if (MenuManager.instance.currentPanel.name == "CharacterPanel")
                {
                    if (selectedButton.GetComponent<WarriorSelectionButtons>())
                    {
                        if ((MenuManager.instance.wichMode == MenuManager.mode.BRAWL) || (MenuManager.instance.wichMode == MenuManager.mode.TRAINING) || (MenuManager.instance.wichMode == MenuManager.mode.HISTORY))
                        {
                            if (MenuManager.instance.numCPU + MenuManager.instance.currentPlayers < 4)
                            {
                                if (selectedButton.GetComponent<WarriorSelectionButtons>().characterName != "null")
                                {

                                    if (MenuManager.instance.selectedPositions[idPlayer + (MenuManager.instance.numCPU + 1)].transform.GetChild(0).GetComponent<Image>().sprite == null)
                                    {
                                        joinCPU(idPlayer + (MenuManager.instance.numCPU + 1));
                                    }
                                    else if (MenuManager.instance.selectedPositions[idPlayer + (MenuManager.instance.numCPU + 2)].transform.GetChild(0).GetComponent<Image>().sprite == null)
                                    {
                                        joinCPU(idPlayer + (MenuManager.instance.numCPU + 2));
                                    }
                                    else if (MenuManager.instance.selectedPositions[idPlayer + (MenuManager.instance.numCPU + 3)].transform.GetChild(0).GetComponent<Image>().sprite == null)
                                    {
                                        joinCPU(idPlayer + (MenuManager.instance.numCPU + 3));
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
    }

    internal void selectMap()
    {
        hasSelectedMap = true;
        mapSelected = selectedButton.GetComponent<MapSelectionButton>().mapName;
        MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[2];
        MenuManager.instance.enableAllreadyWithStyle();
    }

    internal void selectMinigame()
    {
        hasSelectedMinigame = true;
        minigameSelected = selectedButton.GetComponent<MinigameSelectionButton>().minigameName;
        if (minigameSelected == "WaveFight")
        {
            if (MenuManager.instance.currentCharacters == 2)
            {
                minigameSelected = "TeamWaveFight";
            }
        }
        if (minigameSelected == "BossFight")
        {
            if (MenuManager.instance.currentCharacters == 2)
            {
                minigameSelected = "TeamBossFight";
            }
        }
        MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[2];
        MenuManager.instance.enableAllreadyWithStyle();
    }

    private void unselectMap()
    {
        hasSelectedMap = false;
        selectedButton.buttonHighlight();
        MenuManager.instance.disableAllreadyWithStyle();
    }

    private void unselectMinigame()
    {
        hasSelectedMinigame = false;
        selectedButton.buttonHighlight();
        MenuManager.instance.disableAllreadyWithStyle();
    }

    private void unselectCharacter()
    {
        hasSelectedCharacter = false;
        MenuManager.instance.selectedPositions[idPlayer].GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = default(Material);
        selectedButton.buttonHighlight();
        disabeColorPanel();
    }

    public void selectCharacter()
    {
        string[] charactRandom = new string[3];
        charactRandom[0] = "Templar_Player";
        charactRandom[1] = "Valkyrie_Player";
        charactRandom[2] = "Pirate_Player";
        hasSelectedCharacter = true;
        MenuManager.instance.selectedPositions[idPlayer].GetComponent<Image>().color = Color.white;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().color = Color.white;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        if (selectedButton.name == "RandomButton") characterSelected = charactRandom[UnityEngine.Random.Range(0, 3)];
        else characterSelected = selectedButton.GetComponent<WarriorSelectionButtons>().characterName;
        if (Settings.Instance.fightSettings.type != "TeamFight")
        {
            switch (MenuManager.instance.players[idPlayer].selectedButton.name)
            {
                case "TemplarButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.templarMaterials[ColorPanel.GetComponent<ColorPanelController>().currentIdColor]; break;
                case "PirateButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[ColorPanel.GetComponent<ColorPanelController>().currentIdColor]; break;
                case "ValkiryeButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[ColorPanel.GetComponent<ColorPanelController>().currentIdColor]; break;
                case "BknightButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.blackKnightMaterials[ColorPanel.GetComponent<ColorPanelController>().currentIdColor]; break;
            }
            enableColorPanel();
        }
    }

    void setBotButton()
    {
        if (!selectedButton.Equals(selectedButton.bot))
        {
            if (selectedButton.GetComponent<VideoSettingsButton>())
            {
                if(selectedButton.GetComponent<VideoSettingsButton>().isOnDropDown)
                {
                    selectedButton.GetComponentInChildren<Dropdown>().value = selectedButton.GetComponentInChildren<Dropdown>().value +1;
                }
                else
                {
                    selectedButton.buttonHighlightOff();
                    selectedButton = selectedButton.bot;
                    selectedButton.buttonHighlight();
                }
            }
            else
            {
                selectedButton.buttonHighlightOff();
                selectedButton = selectedButton.bot;
                selectedButton.buttonHighlight();
                if (selectedButton.GetComponent<WarriorSelectionButtons>() || selectedButton.GetComponent<MapSelectionButton>() || selectedButton.GetComponent<MinigameSelectionButton>())
                {
                    selectedButton.buttonHighlight(idPlayer);
                }
            }
        }
        else
        {
            SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["wrongSelectButtonSFX"];
            SoundManager.instance.SFXSource.Play();
        }
    }

    void setTopButton()
    {
        if (!selectedButton.Equals(selectedButton.top))
        {
            if (selectedButton.GetComponent<VideoSettingsButton>())
            {
                if (selectedButton.GetComponent<VideoSettingsButton>().isOnDropDown)
                {
                    selectedButton.GetComponentInChildren<Dropdown>().value = selectedButton.GetComponentInChildren<Dropdown>().value - 1;
                }
                else
                {
                    selectedButton.buttonHighlightOff();
                    selectedButton = selectedButton.top;
                    selectedButton.buttonHighlight();
                }
            }
            else
            {
                selectedButton.buttonHighlightOff();
                selectedButton = selectedButton.top;
                selectedButton.buttonHighlight();
                if (selectedButton.GetComponent<WarriorSelectionButtons>() || selectedButton.GetComponent<MapSelectionButton>() || selectedButton.GetComponent<MinigameSelectionButton>())
                {
                    selectedButton.buttonHighlight(idPlayer);
                }
            }
        }
        else
        {
            SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["wrongSelectButtonSFX"];
            SoundManager.instance.SFXSource.Play();
        }
    }

    void setRightButton()
    {
        if (!selectedButton.Equals(selectedButton.right))
        {
            selectedButton.buttonHighlightOff();
            selectedButton = selectedButton.right;
            selectedButton.buttonHighlight();
            if (selectedButton.GetComponent<WarriorSelectionButtons>() || selectedButton.GetComponent<MapSelectionButton>() || selectedButton.GetComponent<MinigameSelectionButton>())
            {
                selectedButton.buttonHighlight(idPlayer);
            }
        }
        else
        {
            if (MenuManager.instance.currentPanel.name == "SoundMenuPanel" || MenuManager.instance.currentPanel.name == "FightSettingsMenuPanel")
            {
                SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["mixerchangeSFX"];
                SoundManager.instance.SFXSource.Play();
            }
            else
            {
                SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["wrongSelectButtonSFX"];
                SoundManager.instance.SFXSource.Play();
            }
        }


    }

    void setLeftButton()
    {
        if (!selectedButton.Equals(selectedButton.left))
        {
            selectedButton.buttonHighlightOff();
            selectedButton = selectedButton.left;
            selectedButton.buttonHighlight();
            if (selectedButton.GetComponent<WarriorSelectionButtons>() || selectedButton.GetComponent<MapSelectionButton>())
            {
                selectedButton.buttonHighlight(idPlayer);
            }
        }
        else
        {
            if (MenuManager.instance.currentPanel.name == "SoundMenuPanel")
            {
                SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["mixerchangeSFX"];
                SoundManager.instance.SFXSource.Play();
            }
            else
            {
                SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["wrongSelectButtonSFX"];
                SoundManager.instance.SFXSource.Play();
            }
        }
    }

    void setCanMoveOn()
    {
        canMove = true;
    }

    void checkMouseInputs()
    {

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            setBotButton();
        }else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            setTopButton();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setLeftButton();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            setRightButton();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            selectedButton.onClickButton();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.instance.returnPanel();
        }
    }

    public void initSelectionPlayer()
    {
        joined = true;
        selectedButton = MenuManager.instance.currentPanel.transform.GetChild(0).GetComponent<Buttons>();
        playerIdentifier = Instantiate(Resources.Load("Identifier/IdentifierP"), selectedButton.gameObject.transform) as GameObject;
        switch(idPlayer)
        {
            case 0: playerIdentifier.GetComponent<Image>().color = Settings.Instance.fightSettings.colors[0];  break;
            case 1: playerIdentifier.GetComponent<Image>().color = Settings.Instance.fightSettings.colors[1]; break;
            case 2: playerIdentifier.GetComponent<Image>().color = Settings.Instance.fightSettings.colors[2]; break;
            case 3: playerIdentifier.GetComponent<Image>().color = Settings.Instance.fightSettings.colors[3]; break;     
        }
        Sprite spr = Resources.Load<Sprite>("Identifier/P"+(idPlayer+1)+"Identifier");
        playerIdentifier.GetComponent<Image>().sprite = spr;
        playerIdentifier.GetComponent<RectTransform>().localPosition = identifierPosition;
        playerIdentifier.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        MenuManager.instance.selectedPositions[idPlayer].GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(1).gameObject.SetActive(false);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().bgCharacter;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().characterPose;
        hasInited = true;
        MenuManager.instance.currentCharacters++;
        MenuManager.instance.currentPlayers++;

    }


    private void unJoinPlayer()
    {
        joined = false;
        MenuManager.instance.currentCharacters--;
        MenuManager.instance.currentPlayers--;
        selectedButton.buttonHighlightOff();
        selectedButton = null;
        playerIdentifier.SetActive(false);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(1).gameObject.SetActive(true);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().sprite = default(Sprite);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = default(Sprite);
        MenuManager.instance.selectedPositions[idPlayer].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);

    }

    private void joinPlayer()
    {
        joined = true;
        MenuManager.instance.currentCharacters++;
        MenuManager.instance.currentPlayers++;
        selectedButton = MenuManager.instance.currentPanel.transform.GetChild(0).GetComponent<Buttons>();
        selectedButton.buttonHighlight();
        playerIdentifier.SetActive(true);
        playerIdentifier.transform.SetParent(selectedButton.transform);
        playerIdentifier.transform.localPosition = identifierPosition;
        MenuManager.instance.selectedPositions[idPlayer].GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.gray;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(1).gameObject.SetActive(false);
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().bgCharacter;
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().characterPose;
    }

    private void joinCPU(int location)
    {
        MenuManager.instance.numCPU++;
        MenuManager.instance.currentCharacters++;
        locationCPU = location;
        MenuManager.instance.selectedPositions[idPlayer+ locationCPU].GetComponent<Image>().color = Color.white;
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).GetComponent<Image>().color = Color.white;
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(1).gameObject.SetActive(false);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().bgCharacter;
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = selectedButton.GetComponent<WarriorSelectionButtons>().characterPose;
        if (MenuManager.instance.wichMode == MenuManager.mode.TRAINING)
        {
            string[] split = selectedButton.GetComponent<WarriorSelectionButtons>().characterName.Split('_');
            string gudName = split[0] + "_Player";
            MenuManager.instance.CPUs.Add(gudName);
        }
        else
        {
            if (selectedButton.name == "RandomButton")
            {
                string[] charactRandom = new string[3];
                charactRandom[0] = "Templar_AI";
                charactRandom[1] = "Valkyrie_AI";
                charactRandom[2] = "Pirate_AI";
                MenuManager.instance.CPUs.Add(charactRandom[UnityEngine.Random.Range(0, 3)]);
            }
            else
            {
                string[] split = selectedButton.GetComponent<WarriorSelectionButtons>().characterName.Split('_');
                string gudName = split[0] + "_AI";
                MenuManager.instance.CPUs.Add(gudName);
            }
        }
    }

    private void unJoinCPU()
    {
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(1).gameObject.SetActive(true);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).GetComponent<Image>().sprite = default(Sprite);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = default(Sprite);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        MenuManager.instance.selectedPositions[idPlayer + locationCPU].GetChild(0).transform.GetChild(0).GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1);
        locationCPU--;
        MenuManager.instance.numCPU--;
        MenuManager.instance.currentCharacters--;
        MenuManager.instance.CPUs.RemoveAt(MenuManager.instance.CPUs.Count - 1);
    }

    void cleanIAinSelectPosition()
    {
        MenuManager.instance.selectedPositions[idPlayer].GetChild(0).GetComponent<Image>().sprite = default(Sprite);
        MenuManager.instance.numCPU--;
        MenuManager.instance.currentCharacters--;
    }

    void enableColorPanel()
    {
        ColorPanel.GetComponent<ColorPanelController>().enable();
        ColorPanel.GetComponent<ColorPanelController>().idPlayer = idPlayer;
    }

    void disabeColorPanel()
    {
        ColorPanel.GetComponent<ColorPanelController>().disable();
    }
}


