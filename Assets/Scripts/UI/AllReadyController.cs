using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AllReadyController : MonoBehaviour {
    private void Start()
    {
        InvokeRepeating("changeImage", 1, 0.1f);
    }

    public void continueByClick()
    {
        if (!MenuManager.instance.players[0].hasSelectedMap)
        {
            MenuManager.instance.checkAllPlayersHaveSelectedChar();
        }
        else
        {
            if (MenuManager.instance.hasAllPlayerSelectedMap()) MenuManager.instance.checkAllPlayersHaveSelectedMap();
        }
        if (!MenuManager.instance.players[0].hasSelectedMinigame)
        {
            MenuManager.instance.checkAllPlayersHaveSelectedChar();
        }
        else
        {
            if (MenuManager.instance.hasAllPlayerSelectedMinigame()) MenuManager.instance.checkAllPlayersHaveSelectedMinigame();
        }
    }
    void changeImage()
    {

        if (InputControl.m_State == InputControl.eInputState.MouseKeyboard)
        {
            if (!MenuManager.instance.players[0].hasSelectedMap)
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[1];
            }
            else
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[3];
            }
        }
        else
        {
            if (!MenuManager.instance.players[0].hasSelectedMap)
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[0];
            }
            else
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[2];
            }
        }
    }
}
