using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateCannon : MonoBehaviour {

    public Attack atk;
    public Character user;
    [SerializeField]
    GameObject cannonBall;

    public void spawnCannonBall()
    {
        GameObject instance = GameObject.Instantiate(cannonBall, this.transform, false);
        instance.transform.parent = null;
        PirateCannonBall proj;
        proj = instance.GetComponent<PirateCannonBall>();
        Physics2D.IgnoreCollision(instance.GetComponent<Collider2D>(), user.HurtBox, true);
        proj.user = user;
        proj.cannon = this.gameObject;
        proj.spawner = true;
        proj.attack = atk;
    }
}
