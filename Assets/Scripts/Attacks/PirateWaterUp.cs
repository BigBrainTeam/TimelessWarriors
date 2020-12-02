using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special Up Attack of Pirate.
/// </summary>
public class PirateWaterUp : MonoBehaviour {
    public Character user;
    public Attack atk;
    public Transform piratePosition;
    bool moving;
    public float speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>())
        {
            Entity ent = collision.GetComponentInParent<Entity>();
            if(ent.state != Utilities.state.Blocking)
            {
                atk.onHit(ent);
            }
            else
            {
                ent.onBlock();
            }
        }
    }
    private void Update()
    {
        if (moving) //makes pirate user move with the attack.
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z);
            user.transform.position = piratePosition.position;
        }
    }
    public void movePirate()
    {
        moving = true;
    }
    public void finish()
    {
        user.transform.parent = null;
        user.Animator.Play("SpecialUp_End");
        Destroy(this.gameObject);
    }
}
