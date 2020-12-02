using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapSelectionButton : Buttons {

    public Vector3 highlightedScale, mainPosition, enterPosition;
    Sequence panelChangeSequence;
    public string mapName;
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
        if (gameObject.name == "RandomMap")
        {
            string[] maps = new string[5];
            maps[0] = "TemplarMap";
            maps[1] = "PirateMap";
            maps[2] = "ValkyrieMap";
            maps[3] = "SamuraiMap";
            maps[4] = "BaseMap";
            mapName = maps[Random.Range(0, 5)];
        }
        MenuManager.instance.players[id].selectMap();
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
