using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MinigameSelectionButton : Buttons {

    public Vector3 highlightedScale, mainPosition, enterPosition;
    Sequence panelChangeSequence;
    public string minigameName;
    void Start()
    {
        highlightedScale = new Vector3(1.08f, 1.08f, 1);
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one;

    }
    public override void buttonHighlight()
    {
        base.buttonHighlight();
        gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["selectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }

    public override void buttonHighlightOff()
    {
        base.buttonHighlightOff();
        if (!highlighted)
        {
            gameObject.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);
        }
    }

    public override void onClickButton(int id)
    {
        if (gameObject.name == "RandomMinigame")
        {
            string[] mgames = new string[5];
            mgames[0] = "WaveFight";
            mgames[1] = "BossFight";
            minigameName = mgames[Random.Range(0, 5)];
        }
        MenuManager.instance.players[id].selectMinigame();
        //gameObject.GetComponent<Image>().color = Color.gray;
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["clickSelectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }

    public void initPanelChangeSequence()
    {
        panelChangeSequence = DOTween.Sequence();
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(enterPosition, 0.3f));
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(mainPosition, 0.3f));
    }

    public override void buttonHighlight(int id)
    {
        base.buttonHighlight();
        gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f);
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["selectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
        MenuManager.instance.players[id].playerIdentifier.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        MenuManager.instance.players[id].playerIdentifier.transform.SetParent(MenuManager.instance.players[id].selectedButton.transform);
        MenuManager.instance.players[id].playerIdentifier.transform.localPosition = MenuManager.instance.players[id].identifierPosition;
    }

    public override void buttonHighlightOff(int id)
    {
        base.buttonHighlightOff();
        if (!highlighted)
        {
            gameObject.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f);
        }
        MenuManager.instance.players[id].playerIdentifier.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        MenuManager.instance.players[id].playerIdentifier.transform.SetParent(MenuManager.instance.players[id].selectedButton.transform);
        MenuManager.instance.players[id].playerIdentifier.transform.localPosition = MenuManager.instance.players[id].identifierPosition;
    }
}
