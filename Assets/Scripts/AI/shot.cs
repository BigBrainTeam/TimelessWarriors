using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shot : MonoBehaviour
{
    public Entity alien;
    public float direction;
    Attack bulletshot;
    //internal float direccion;
   
    // Use this for initialization
    void Start()
    {
        //Con la direccion del alien el disparo avanzara en está hasta salir de la vision del jugador o golpearlo
        bulletshot = new bulletshot(alien);
        if (direction > 0)
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
        else if (direction < 0 )
        {
            transform.localScale = new Vector3(-2, 2, 2);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(direction * 10, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11 && collision.gameObject.transform.parent.gameObject.layer != 17)
        {
            Destroy(this.gameObject);
            bulletshot.onHit(collision.GetComponentInParent<Entity>());
        }
    }
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
public class bulletshot : Attack{ 
        
      public bulletshot(Entity user) : base(user)
    {
        name = "bulletshot";
        damage = 3;
        knockBack = 5;
        type = "Side";
    }
}