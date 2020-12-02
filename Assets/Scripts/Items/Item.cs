using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Item : MonoBehaviour, Hitter {

    [SerializeField]protected float damage, knockBack, velocity;
    protected Rigidbody2D rbd;
    protected Animator anim;
    protected Collider2D col;
    protected enum type { explosive, bullet, hook}
    protected type itemType;

    protected virtual void initializeComponents()
    {
        rbd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }


    public float getKnockBack()
    {
        return knockBack;
    }
    public float getDamage()
    {
        return damage;
    }

    public float getVelocity()
    {
        return velocity;
    }
    public Rigidbody2D getRigidBody()
    {
        return rbd;
    }
    public virtual void itemAction() { }
    public virtual void itemAction(Entity target) { }
    public virtual void itemAction(Collider2D c) {}

    protected void Start()
    {
        initializeComponents();
    }

    virtual protected void Update()
    {

    }

    public virtual void knockBackEntity(Entity target, float totalKnockBack)
    {
        switch (itemType)
        {
            case type.explosive: Utilities.AddExplosionForce(target.RigidBody, damage, transform.position, 4, this); break;
            case type.bullet: Utilities.calculateDirectionForce(target, gameObject.GetComponent<ThrowableItem>()); break;
        }
    }

    public void onHit(float frames)
    {
        throw new NotImplementedException();
    }
    
}
