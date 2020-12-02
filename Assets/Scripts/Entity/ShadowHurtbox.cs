using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowHurtbox : MonoBehaviour {

    TemplarShadow shadow;
	// Use this for initialization
	void Start () {
        shadow = GetComponentInParent<TemplarShadow>();
        Collider2D hbox = this.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(shadow.blackTemplar.transform.GetChild(3).GetComponent<Collider2D>(), hbox);
        Physics2D.IgnoreCollision(shadow.GetComponent<Collider2D>(), hbox);
        if (GameManager.Instance.currentFight is TeamFight)
        {
            Entity friend = ((TeamFight)GameManager.Instance.currentFight).getTeammate(shadow.blackTemplar.player);
            Physics2D.IgnoreCollision(hbox, friend.transform.GetChild(3).GetComponent<Collider2D>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Invoke("resetS", 0.2f);
    }

    private void resetS()
    {
        shadow.resetComps();
    }
}
