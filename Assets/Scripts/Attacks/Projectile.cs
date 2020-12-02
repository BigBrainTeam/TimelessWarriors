using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Projectile : MonoBehaviour {

    public Entity user;
    protected Rigidbody2D rbd;
    [SerializeField]
    public Utilities.direction direction;
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected bool rotates;
    public Attack attack;
    public GameObject hitEffect;
    public GameObject spawnEffect;


    virtual protected void Start()
    {
        rbd = GetComponent<Rigidbody2D>();
        if (spawnEffect) Instantiate(spawnEffect, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        switch (direction)
        {
            case Utilities.direction.Up:
                {
                    rbd.velocity = new Vector2(0, speed);
                }
                break;
            case Utilities.direction.Down:
                {
                    rbd.velocity = new Vector2(0, -speed);
                }
                break;
            case Utilities.direction.Left:
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    rbd.velocity = new Vector2(-speed, 0);
                }
                break;
            case Utilities.direction.Right:
                {
                    rbd.velocity = new Vector2(speed, 0);
                }
                break;
        }
    }

    protected void Update()
    {
        if (rotates)
        {
            if (direction == Utilities.direction.Left) transform.Rotate(new Vector3(0f, 0f, speed));
            else transform.Rotate(new Vector3(0f, 0f, -speed));
        }
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>())
        {
            Entity ent = collision.GetComponentInParent<Entity>();
            if(ent.state != Utilities.state.Blocking)
            {
                attack.onHit(ent, this.gameObject);
                if (hitEffect) Instantiate(hitEffect, new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), Quaternion.identity);
            }
            else
            {
                ent.onBlock();
            }
        }
        GameObject.Destroy(this.gameObject);
    }
    protected void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
