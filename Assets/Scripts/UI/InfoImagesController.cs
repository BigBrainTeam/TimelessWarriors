using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class InfoImagesController : MonoBehaviour {
    public Vector3 mainPosition, enterPosition;
    public string idname;
    public Sprite mouseImg, controllerImg;
    Sequence panelChangeSequence;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(MenuManager.instance.currentPanel.name == "MainMenuPanel")
        {
            if(idname == "InfoB2")
            {
                disableInfoB();
            }
            else gameObject.GetComponent<Image>().enabled = true;
        }else
        {
            if (idname == "InfoB2")
            {
                if(gameObject.GetComponent<Image>().enabled == false)
                {
                    Invoke("enableInfoB", 0.3f);
                }
            }
        }

        if (idname == "InfoB2")
        {
            if(InputControl.m_State == InputControl.eInputState.MouseKeyboard)
            {
                gameObject.GetComponent<Image>().sprite = mouseImg;
            }
            else
            {
                gameObject.GetComponent<Image>().sprite = controllerImg;
            }
        }

        if (idname == "InfoB1")
        {
            if (InputControl.m_State == InputControl.eInputState.MouseKeyboard)
            {
                gameObject.GetComponent<Image>().enabled = false;
                gameObject.GetComponent<Button>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Image>().enabled = true;
                gameObject.GetComponent<Button>().enabled = true;
            }
        }


    }

    public void initPanelChangeSequence()
    {
        panelChangeSequence = DOTween.Sequence();
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(enterPosition, 0.3f));
        panelChangeSequence.Append(gameObject.GetComponent<RectTransform>().DOLocalMove(mainPosition, 0.3f));
    }

    void enableInfoB()
    {
        gameObject.GetComponent<Image>().enabled = true;
        gameObject.GetComponent<Button>().enabled = true;
    }

    void disableInfoB()
    {
        gameObject.GetComponent<Image>().enabled = false;
        gameObject.GetComponent<Button>().enabled = false;
    }

    public void OnClickBack()
    {
        MenuManager.instance.returnPanel();
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["backSelectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }

    public void OnClickSelect()
    {
        MenuManager.instance.players[0].selectedButton.onClickButton();
    }
}
