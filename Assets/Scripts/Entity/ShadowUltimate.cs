using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowUltimate : MonoBehaviour {

    public Entity user;
    public void stopUltimate()
    {
        user.Sprite.enabled = true;
        user.stopAttacking();
        user.HurtBox.enabled = true;
        Destroy(this.gameObject);
    }
}
