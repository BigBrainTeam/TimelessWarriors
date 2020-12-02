using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for each Attack Controller of Characters.
/// </summary>
public class AttacksController : MonoBehaviour {
    /// <summary>
    /// Possible attacks
    /// </summary>
    public Attack[] attacks;
    public Utilities.attackType currentAttack;
    public Entity user;

    public void Start()
    {
        initializeComponents();
    }
       
    protected virtual void initializeComponents()
    {
        currentAttack = 0;
    }
    public float getMaxCharge()
    {
        return (attacks[(int)currentAttack].damage / 2);
    }
    public void startAttack(Utilities.attackType attack)
    {
        attacks[(int)attack].doAttack();
        currentAttack = attack;
    }
    /// <summary>
    /// Called when Character has Hit something.
    /// </summary>
    /// <param name="collision"></param>
    public void hit(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>() != null) //if hits an Entity.
        {
            Entity target = collision.GetComponentInParent<Entity>();
            if (target.state != Utilities.state.Blocking)//not blocking, realize hit.
            {
                if (currentAttack != Utilities.attackType.None) attacks[(int)currentAttack].onHit(target);
            }
            else
            {
                if(user.state != Utilities.state.Ultimate)// if blocking and not ultimate, parry effect(stun).
                {
                    user.stopAttacking();
                    user.state = Utilities.state.Hit;
                    user.RigidBody.velocity = new Vector2(0, 0);
                    user.Animator.Play("Hit");
                    user.StartCoroutine(user.hitStun(25f));
                }
                ((Character)target).onBlock();
            } 
        }
        else if (collision.GetComponentInParent<Ball>())
        {
            Ball ballItem = collision.GetComponentInParent<Ball>();
            if (currentAttack != Utilities.attackType.None)
            {
                float damage = attacks[(int)currentAttack].damage;
                float attackKnock = attacks[(int)currentAttack].knockBack;
                float knockBack = Utilities.calculateKnockBack(50, damage, ballItem.getRigidBody().mass, attackKnock);
                ballItem.addForceByAttack(user, knockBack);
            }
        }
    }

    public void hit(Collider2D collision, GameObject projectile) //if projectile hits
    {
        if (collision.GetComponentInParent<Entity>() != null)
        {
            Entity target = collision.GetComponentInParent<Entity>();
            if (target.state != Utilities.state.Blocking)
            {
                if (currentAttack != Utilities.attackType.None) attacks[(int)currentAttack].onHit(target, projectile);
            }
            else
            {
                ((Character)target).onBlock();
                //possible reflect implementation
            }
        }
        else if (collision.GetComponentInParent<Ball>())
        {
            Ball ballItem = collision.GetComponentInParent<Ball>();
            if (currentAttack != Utilities.attackType.None)
            {
                float damage = attacks[(int)currentAttack].damage;
                float attackKnock = attacks[(int)currentAttack].knockBack;
                float knockBack = Utilities.calculateKnockBack(50, damage, ballItem.getRigidBody().mass, attackKnock);
                ballItem.addForceByAttack(user, knockBack);
            }
        }
    }
}
