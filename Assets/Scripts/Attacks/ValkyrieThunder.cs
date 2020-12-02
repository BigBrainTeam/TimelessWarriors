using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyrieThunder : MonoBehaviour {
    public Character user;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>())
        {
            user.attacks.currentAttack = Utilities.attackType.SpecialDown;
            user.attacks.hit(collision);
        }
    }
    public void finishThunder()
    {
        Destroy(this.gameObject);
    }
}
