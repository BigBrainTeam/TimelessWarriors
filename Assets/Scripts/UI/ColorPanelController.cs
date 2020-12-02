using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ColorPanelController : MonoBehaviour
{


    public int idPlayer, currentIdColor;
    GameObject currentColor;
    public Sprite selectedImage, defaultSelectedImage, normalImage, defaultNormalImage;
    GameObject[] colors = new GameObject[4];
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < transform.GetChild(2).transform.childCount; i++)
        {
            colors[i] = transform.GetChild(2).transform.GetChild(i).gameObject;
        }
        currentIdColor = 0;
        currentColor = colors[currentIdColor];
        colorHighlight();
    }

    public void enable()
    {
        gameObject.GetComponent<RectTransform>().DOScaleX(1, 0.1f);
    }

    public void disable()
    {
        gameObject.GetComponent<RectTransform>().DOScaleX(0, 0.1f);
    }

    public void changeColorLeft()
    {
        if (currentIdColor <= 0)
        {
            currentIdColor = colors.Length - 1;
            colorHighlightOff();
            currentColor = colors[currentIdColor];
            colorHighlight();
        }
        else
        {
            currentIdColor--;
            colorHighlightOff();
            currentColor = colors[currentIdColor];
            colorHighlight();
        }

        switch(MenuManager.instance.players[idPlayer].selectedButton.name)
        {
            case "TemplarButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.templarMaterials[currentIdColor]; break;
            case "PirateButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[currentIdColor]; break;
            case "ValkiryeButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[currentIdColor]; break;
            case "BknightButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.blackKnightMaterials[currentIdColor]; break;
        }
    }

    public void changeColorRight()
    {
        if (currentIdColor >= colors.Length - 1)
        {
            currentIdColor = 0;
            colorHighlightOff();
            currentColor = colors[currentIdColor];
            colorHighlight();
        }
        else
        {
            currentIdColor++;
            colorHighlightOff();
            currentColor = colors[currentIdColor];
            colorHighlight();
        }

        switch (MenuManager.instance.players[idPlayer].selectedButton.name)
        {
            case "TemplarButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.templarMaterials[currentIdColor]; break;
            case "PirateButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.pirateMaterials[currentIdColor]; break;
            case "ValkiryeButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.valkyrieMaterials[currentIdColor]; break;
            case "BknightButton": MenuManager.instance.selectedPositions[idPlayer].GetChild(0).transform.GetChild(0).GetComponent<Image>().material = GameManager.Instance.blackKnightMaterials[currentIdColor]; break;
        }
    }

    public void selectColor(int idPlayer)
    {
        MenuManager.instance.players[idPlayer].hasSelectedColor = true;
        if (currentIdColor == 0) currentColor.GetComponent<Image>().sprite = defaultSelectedImage;
        else currentColor.GetComponent<Image>().sprite = selectedImage;

        gameObject.GetComponent<RectTransform>().DOScaleX(0, 0.1f);

        if (MenuManager.instance.wichMode == MenuManager.mode.BRAWL)
        {
            if(Settings.Instance.fightSettings.type == "TeamFight")
            {
                if (MenuManager.instance.hasAllPlayerSelectedColor() && MenuManager.instance.currentCharacters == 4)
                {
                    MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[0];
                    MenuManager.instance.enableAllreadyWithStyle();
                }
            }else if (MenuManager.instance.hasAllPlayerSelectedColor() && MenuManager.instance.currentCharacters > 1)
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[0];
                MenuManager.instance.enableAllreadyWithStyle();
            }
        }
        else
        {
            if (MenuManager.instance.hasAllPlayerSelectedColor())
            {
                MenuManager.instance.AllreadyImage.transform.GetChild(0).GetComponent<Image>().sprite = MenuManager.instance.allreadyImages[0];
                MenuManager.instance.enableAllreadyWithStyle();
            }
        }


    }

    public void unselectColor(int idPlayer)
    {
        MenuManager.instance.players[idPlayer].hasSelectedColor = false;
        if (currentIdColor == 0) currentColor.GetComponent<Image>().sprite = defaultNormalImage;
        else currentColor.GetComponent<Image>().sprite = normalImage;
        if (MenuManager.instance.hasAllPlayerSelected() && MenuManager.instance.AllreadyImage.GetComponent<RectTransform>().localScale == Vector3.one)
        {
            MenuManager.instance.disableAllreadyWithStyle();
        }

        gameObject.GetComponent<RectTransform>().DOScaleX(1, 0.1f);
    }

    public void colorHighlight()
    {
        currentColor.GetComponent<RectTransform>().DOScale(new Vector3(1.1f, 1.1f, 1), 0.2f);
        //currentColor.GetComponent<Image>().DOColor();
    }

    public void colorHighlightOff()
    {
        currentColor.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1), 0.2f);
        //currentColor.GetComponent<Image>().DOColor();
    }
}

