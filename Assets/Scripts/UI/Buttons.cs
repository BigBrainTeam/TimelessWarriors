using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour {
    public Buttons right, left, top, bot;
    public bool highlighted;   

    public virtual void onClickButton() { }
    public virtual void onClickButton(int id) { }
    public virtual void buttonHighlight(int id) { highlighted = true; }
    public virtual void buttonHighlightOff(int id) {
        int canHighlightOff = 0;
        for (int i = 0; i < MenuManager.instance.players.Count; i++)
        {
            if (MenuManager.instance.players[i].selectedButton == this)
            {
                canHighlightOff++;
            }
        }
        if (canHighlightOff > 1)
        {
            highlighted = true;
        }
        else
        {
            highlighted = false;
        }
    }
    public virtual void buttonHighlight() {
        highlighted = true;
    }
    public virtual void buttonHighlightOff() {
        int canHighlightOff = 0;
        for (int i = 0; i < MenuManager.instance.players.Count; i++)
        {
            if (MenuManager.instance.players[i].selectedButton == this)
            {
                canHighlightOff++;
            }
        }
        if (canHighlightOff > 1)
        {
            highlighted = true;
        }
        else
        {
            highlighted = false;
        }

    }
}
