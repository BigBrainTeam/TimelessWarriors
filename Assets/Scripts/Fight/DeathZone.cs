using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Entity>())
        {
            Entity ent = collision.GetComponent<Entity>();
            if (ent.hasItem) {
                ent.hasItem = false;
                GameManager.Instance.currentFight.onObjectDestroyed(ent.grabbedItem.gameObject);
                Destroy(ent.grabbedItem.gameObject);
            } 
            GameManager.Instance.currentFight.onBorderCollision(ent);
        }
        else if (collision.gameObject.GetComponent<Item>())
        {
            GameManager.Instance.currentFight.onObjectDestroyed(collision.gameObject);
            Destroy(collision.gameObject);
        }
   
    }
}
