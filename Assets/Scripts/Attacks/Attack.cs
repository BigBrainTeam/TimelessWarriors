using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Base class for all the Attacks.
/// </summary>
public class Attack : Hitter {
    public float damage, knockBack, stunTime, initialDamage;
    public bool chargeable, stops, stuns;
    protected string type, name;
    public string animationName;
    public Entity user, target;
    public List<Entity> hittedEntities;
    public GameObject effect;
    public AudioClip audioclip;

    public Attack(Entity user, float damage, float knockBack)
    {
        this.user = user;
        this.damage = damage;
        this.knockBack = knockBack;
        hittedEntities = new List<Entity>();
    }
    public Attack(Entity user)
    {
        this.user = user;
        hittedEntities = new List<Entity>();
    }
    /// <summary>
    /// Starts the attack.
    /// </summary>
    public virtual void doAttack()
    {
        user.Animator.Play(animationName);
        if (stops) {
            user.RigidBody.velocity = new Vector2(0, 0);
            user.RigidBody.gravityScale = 0;
        }

        if(audioclip)
        {
            user.GetComponent<AudioSource>().clip = audioclip;
            user.GetComponent<AudioSource>().Play();
        }
    }
    /// <summary>
    /// Called when an Entity is hit by the Attack.
    /// </summary>
    /// <param name="target">hitted Entity</param>
    public virtual void onHit(Entity target)
    {
        if (canBeHit(target))
        {
            //Set up the new state of hitted target.
            target.State = Utilities.state.Hit;
            hittedEntities.Add(target);
            target.Animator.Play("Hit");
            if (target.State == Utilities.state.Edge) target.BoxCollider.enabled = true;
            target.RigidBody.gravityScale = target.getGravityAmount();
            target.RigidBody.velocity = new Vector2(0, 0);

            //Deals the damage to target. 
            float totalDamage = damage;
            float startingHealth = target.getLife();
            if (chargeable)
            {
                totalDamage = damage + ((user.getChargeAmount() * damage) / 2);
                user.setAttackCharge(0);
            }
            target.receiveDamage(totalDamage);
            float lostLife = target.getMaxLife() - target.getLife();
            float mappedLife = Map(lostLife, target.getMaxLife());
            if (user is Character) chargeEnergy(user, totalDamage / 2);


            if (stuns)// If Attack stuns.
            {
                if (stuns) target.StartCoroutine(target.hitStun(stunTime));
                else target.StartCoroutine(target.hitStun(60f));
            }
            else
            {
                if(target.getLife() == 0)//finisher hit
                {
                    target.StartCoroutine(target.receiveHit(25, 80, this));
                }
                else
                {
                    //normal hit
                    float totalKnockBack = Utilities.calculateKnockBack(mappedLife, totalDamage, target.RigidBody.mass, knockBack);
                    target.StartCoroutine(target.receiveHit(totalDamage, totalKnockBack, this));
                }
            }
            user.StartCoroutine(user.hitLag(totalDamage, false));
            GameManager.Instance.currentFight.onEntityHit(user, target, (int)totalDamage);
        }       
    }

    private float Map(float value, float inMax)
    {
        return (value - 0) * (100 - 0) / (inMax - 0) + 0;
    }
    /// <summary>
    /// Check if target can be hit. Prevents double hits from same attack.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool canBeHit(Entity target)
    {
        foreach(Entity e in hittedEntities)
        {
            if (e == target) return false;
        }
        return true;
    }

    public virtual void onHit(Entity target, GameObject projectile) { }
    public string getType()
    {
        return type;
    }
    public string getName()
    {
        return name;
    }
    /// <summary>
    /// Deals knockback to the target.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="totalKnockBack"></param>
    public virtual void knockBackEntity(Entity target, float totalKnockBack)
    {
        target.RigidBody.velocity = new Vector2(0, 0);
        switch (type)
        {
            case "Side": sideKnockBackEntity(target, totalKnockBack, user.facingDirection); break;
            case "Up": upKnockBackEntity(target, totalKnockBack); break;
            case "Down": downKnockBackEntity(target, totalKnockBack); break;
            case "Diagonal": diagonalKnockBackEntity(target, totalKnockBack, user.facingDirection); break;
        }
    }

    protected void chargeEnergy(Entity c, float charge)
    {
        Character character = (Character)c;
        character.increaseEnergy(charge);
    }

    private void diagonalKnockBackEntity(Entity target, float totalKnockBack, Utilities.direction direction)
    {
        float camf = 0.1f;
        if (totalKnockBack > 30) camf = 0.5f;
        user.cam.transform.DOShakePosition(0.1f, camf, 1, 90, false, true);

        target.RigidBody.velocity = new Vector2(0, 0);
        if (direction == Utilities.direction.Right) target.RigidBody.AddForce(new Vector2(totalKnockBack/2, totalKnockBack / 2), ForceMode2D.Impulse);
        else target.RigidBody.AddForce(new Vector2(-totalKnockBack/2, totalKnockBack / 2), ForceMode2D.Impulse);
    }

    protected void sideKnockBackEntity(Entity target, float totalKnockBack, Utilities.direction direction)
    {
        float camf = 0.1f;
        if (totalKnockBack > 30) camf = 0.5f;
        user.cam.transform.DOShakePosition(0.1f, camf, 1, 90, false, true);

        target.RigidBody.velocity = new Vector2(0, 0);
        float forceY = 0;
        if (target.State == Utilities.state.Air)
        {
            Vector2 forceDirection = (target.transform.position - user.transform.position);
            if (forceDirection.y > 0.5f) forceY = totalKnockBack * 0.5f;
            else if (forceDirection.y < -0.5f) forceY = totalKnockBack * -0.5f;
            else forceY = forceDirection.y * totalKnockBack;
        }
        else forceY = totalKnockBack / 2;

        if (direction == Utilities.direction.Right) target.RigidBody.AddForce(new Vector2(totalKnockBack, forceY), ForceMode2D.Impulse);
        else target.RigidBody.AddForce(new Vector2(-totalKnockBack, forceY), ForceMode2D.Impulse);        
        target.StartCoroutine(target.hitStun(totalKnockBack));
    }

    protected void upKnockBackEntity(Entity target, float totalKnockBack)
    {
        float camf = 0.1f;
        if (totalKnockBack > 30) camf = 0.5f;
        user.cam.transform.DOShakePosition(0.1f, camf, 1, 90, false, true);

        target.RigidBody.velocity = new Vector2(0, 0);

        Vector2 forceDirection = (target.transform.position - user.transform.position);
        float forceX;
        if (forceDirection.x > 0.1f) forceX = totalKnockBack * 0.1f;
        else if (forceDirection.x < -0.1f) forceX = totalKnockBack * -0.1f;
        else forceX = forceDirection.x * totalKnockBack;
        target.RigidBody.AddForce(new Vector2(forceX, totalKnockBack), ForceMode2D.Impulse);

        target.StartCoroutine(target.hitStun(totalKnockBack));
    }

    protected void downKnockBackEntity(Entity target, float totalKnockBack)
    {
        float camf = 0.1f;
        if (totalKnockBack > 30) camf = 0.5f;
        user.cam.transform.DOShakePosition(0.1f, camf, 1, 90, false, true);
        target.RigidBody.velocity = new Vector2(0, 0);

        Vector2 forceDirection = (target.transform.position - user.transform.position);
        float forceX;
        if (forceDirection.x > 0.5f) forceX = totalKnockBack * 0.5f;
        else if (forceDirection.x < -0.5f) forceX = totalKnockBack * -0.5f;
        else forceX = forceDirection.x * totalKnockBack;
        target.RigidBody.AddForce(new Vector2(forceX, -totalKnockBack), ForceMode2D.Impulse);

        target.StartCoroutine(target.hitStun(totalKnockBack));
    }

    public void onHit(float frames)
    {
        
    }

    internal void resetHittedEntities()
    {
        hittedEntities.Clear();
    }
}
/// <summary>
/// Base class for Side Attacks.
/// </summary>
public class BasicSideAttack: Attack {

    public BasicSideAttack(Entity user, float damage, float knockBack, bool air, AudioClip sound):base(user, damage, knockBack) {
        if (air)
        {
            name = "AirSide";
            animationName = "BasicAirSide";
            stops = false;
            chargeable = false;
        }
        else
        {
            name = "FloorSide";
            animationName = "BasicFloorSide";
            stops = true;
            chargeable = true;
        }
        type = "Side";
        audioclip = sound;
    }
    public BasicSideAttack(Entity user, float damage, float knockBack, bool air) : base(user, damage, knockBack)
    {
        if (air)
        {
            name = "AirSide";
            animationName = "BasicAirSide";
            stops = false;
            chargeable = false;
        }
        else
        {
            name = "FloorSide";
            animationName = "BasicFloorSide";
            stops = true;
            chargeable = true;
        }
        type = "Side";
    }
}
/// <summary>
/// Base class for Up Attacks
/// </summary>
public class BasicUpAttack : Attack
{

    public BasicUpAttack(Entity user, float damage, float knockBack, bool air, AudioClip sound) : base(user, damage, knockBack)
    {
        if (air)
        {
            name = "AirUp";
            animationName = "BasicAirUp";
            stops = false;
            chargeable = false;
        }
        else
        {
            name = "FloorUp";
            animationName = "BasicFloorUp";
            stops = true;
            chargeable = true;
        }
        type = "Up";
        audioclip = sound;
    }

    public BasicUpAttack(Entity user, float damage, float knockBack, bool air) : base(user, damage, knockBack)
    {
        if (air)
        {
            name = "AirUp";
            animationName = "BasicAirUp";
            stops = false;
            chargeable = false;
        }
        else
        {
            name = "FloorUp";
            animationName = "BasicFloorUp";
            stops = true;
            chargeable = true;
        }
        type = "Up";
    }
}
/// <summary>
/// Base class for Down Attacks
/// </summary>
public class BasicDownAttack : Attack
{

    public BasicDownAttack(Entity user, float damage, float knockBack, AudioClip sound) : base(user, damage, knockBack)
    {
        name = "AirDown";
        animationName = "BasicAirDown";
        stops = false;
        type = "Side";
        audioclip = sound;
    }
    public BasicDownAttack(Entity user, float damage, float knockBack) : base(user, damage, knockBack)
    {
        name = "AirDown";
        animationName = "BasicAirDown";
        stops = false;
        type = "Side";
    }
}
/// <summary>
/// Base class for Moving Attacks.
/// </summary>
public class BasicMovingAttack : Attack {

    public BasicMovingAttack(Entity user, float damage, float knockBack) : base(user, damage, knockBack)
    {
        name = "MovingAttack";
        animationName = "MovingAttack";
        stops = false;
        type = "Side";
    }
    public override void onHit(Entity _target)
    {
        base.onHit(_target);
    }
    public override void doAttack()
    {
        base.doAttack();
        if (user.facingDirection == Utilities.direction.Right) user.RigidBody.velocity = new Vector2(18f, 0f);
        else user.RigidBody.velocity = new Vector2(-18f, 0f);
    }
}