using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ItemSelectionButton : Buttons {

    public Vector3 highlightedScale, mainPosition, enterPosition;
    Sequence panelChangeSequence;
    public bool on;
    public string name;

    void Start()
    {
        highlightedScale = new Vector3(1.08f, 1.08f, 1);
        gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        setButtonOn();
        //image = gameObject.GetComponent<Image>();
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

    public override void onClickButton()
    {
        changeButtonState();
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

    public void changeButtonState()
    {
        if (on) setButtonOff();
        else setButtonOn();
    }

    public void setButtonOff()
    {
        on = false;
        gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.red;
        gameObject.transform.GetChild(1).GetComponentInChildren<Text>().text = "Off";
    }

    void setButtonOn()
    {
        on = true;
        gameObject.transform.GetChild(1).GetComponent<Image>().color = Color.green;
        gameObject.transform.GetChild(1).GetComponentInChildren<Text>().text = "On";
    }
}
