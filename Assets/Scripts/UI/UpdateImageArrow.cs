using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateImageArrow : MonoBehaviour {

    public Sprite ArrowR, ArrowL, LB, RB;
    void Update()
    {
        if(InputControl.m_State == InputControl.eInputState.MouseKeyboard)
        {
            if(name == "RightArrow")
            {
                gameObject.GetComponent<Image>().sprite = ArrowR;
            }else if(name == "LeftArrow")
            {
                gameObject.GetComponent<Image>().sprite = ArrowL;
            }
        }
        else
        {
            if (name == "RightArrow")
            {
                gameObject.GetComponent<Image>().sprite = RB;
            }
            else if (name == "LeftArrow")
            {
                gameObject.GetComponent<Image>().sprite = LB;
            }
        }
    }
}
