using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : Buttons {

    public Vector3 highlightedScale, mainPosition, enterPosition;
    Sequence panelChangeSequence;

    public override void buttonHighlight()
    {
        switch (gameObject.name)
        {
            case "Local":
                highlightedScale = new Vector3(1.08f, 1.08f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Online":
                highlightedScale = new Vector3(1.08f, 1.08f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Profile":
                highlightedScale = new Vector3(1.07f, 1.07f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Shop":
                highlightedScale = new Vector3(1.07f, 1.07f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Options":
                highlightedScale = new Vector3(1.11f, 1.08f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f).SetUpdate(true);
                break;
            case "Exit":
                highlightedScale = new Vector3(1.2f, 1.06f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "History":
                highlightedScale = new Vector3(1.07f, 1.06f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);                
                break;
            case "Tournament":
                highlightedScale = new Vector3(1.05f, 1.05f, 1.05f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Training":
                highlightedScale = new Vector3(1.055f, 1.08f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "Brawl":
                highlightedScale = new Vector3(1.07f, 1.07f, 1.05f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            case "MiniGames":
                highlightedScale = new Vector3(1.08f, 1.06f, 1.05f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
                break;
            default:
                highlightedScale = new Vector3(1.08f, 1.08f, 1.08f);
                gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f).SetUpdate(true);
                break;
        }
        base.buttonHighlight();

        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["selectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }

    public override void buttonHighlightOff()
    {
       gameObject.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetUpdate(true);
       base.buttonHighlightOff();
    }

    public override void onClickButton()
    {
        switch(gameObject.name)
        {
            case "Local":goToLocalMenu(); break;
            case "Online": goToOnlineMenu(); break;
            case "Profile": break;
            case "Shop": break;
            case "Options":goToOptionsMenu(); break;
            case "Exit": Application.Quit(); break;
            case "History":
                Settings.Instance.fightSettings.nPlayersCanJoin = 2;
                Settings.Instance.fightSettings.type = "LifeFight";

                StartCoroutine(MenuManager.instance.loadSceneWithFade("CharacterSelection"));
                MenuManager.instance.wichMode = MenuManager.mode.HISTORY; break;
            case "MiniGames":
                Settings.Instance.fightSettings.nPlayersCanJoin = 2;
                StartCoroutine(MenuManager.instance.loadSceneWithFade("CharacterSelection"));
                MenuManager.instance.wichMode = MenuManager.mode.MINIGAMES; break;
            case "Sound": goToSoundMenu(); break;
            case "Video": goToVideoMenu(); break;
            case "Control": goToControlMenu(); break;
            case "MainMenu": {
                    Time.timeScale = 1;
                    GameObject.Destroy(GameManager.Instance.currentFight.gameObject);
                    if(GameManager.Instance.arcadeInstance) GameObject.Destroy(GameManager.Instance.arcadeInstance.gameObject);
                    SceneManager.LoadScene("MainMenu");
                } break;
            case "Brawl":
                Settings.Instance.fightSettings.nPlayersCanJoin = 4;
                Settings.Instance.fightSettings.type = "LifeFight";
                StartCoroutine(MenuManager.instance.loadSceneWithFade("CharacterSelection"));
                MenuManager.instance.wichMode = MenuManager.mode.BRAWL;
                break;
            case "Training":
                Settings.Instance.fightSettings.nPlayersCanJoin = 4;
                Settings.Instance.fightSettings.type = "TrainingFight";
                StartCoroutine(MenuManager.instance.loadSceneWithFade("CharacterSelection"));
                MenuManager.instance.wichMode = MenuManager.mode.TRAINING;
                break;
            case "Resume": GameManager.Instance.currentFight.resume(); break;
            case "Apply": MenuManager.instance.activePanel("CharacterPanel");
                GetComponentInParent<FightSettingsController>().saveCombatSettings();
                break;
            case "Items": MenuManager.instance.activePanel("ItemsMenuPanel"); break;
            case "EditSettings": MenuManager.instance.activePanel("FightSettingsMenuPanel"); break;
            case "DisableAll":disableAllObjects(); break;
            case "ScreenMode": gameObject.GetComponentInChildren<Toggle>().Select();break;
        }

        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["clickSelectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }


    // Use this for initialization

    void goToLocalMenu()
    {      
        MenuManager.instance.activePanel("LocalMenuPanel");;
    }

    void goToOnlineMenu()
    {
        MenuManager.instance.activePanel("OnlineMenuPanel");
    }

    void goToOptionsMenu()
    {
        MenuManager.instance.activePanel("OptionsMenuPanel");
    }
    void goToSoundMenu()
    {
        MenuManager.instance.activePanel("SoundMenuPanel");
    }

    void goToVideoMenu()
    {
        MenuManager.instance.activePanel("VideoMenuPanel");
    }

    void goToControlMenu()
    {
        MenuManager.instance.activePanel("ControlMenuPanel");
    }

    public void initPanelChangeSequence()
    {
        panelChangeSequence = DOTween.Sequence();
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(enterPosition, 0.25f)).SetUpdate(true);
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(mainPosition, 0.25f)).SetUpdate(true);
    }

    void disableAllObjects()
    {
        if (MenuManager.instance.players[0].selectedButton.name == "DisableAll")
        {
            ItemSelectionButton[] items;
            items = FindObjectsOfType<ItemSelectionButton>();
            foreach (ItemSelectionButton isb in items)
            {
                isb.setButtonOff();
            }
        }
    }
}
