using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for Templar, controls charges and Ultimate states.
/// </summary>
public class Templar : Speciality {

    int charges;
    Image[] chargesImages;
    Vector3 chargePosition, topPoint, direction, rightPoint, leftPoint;
    public int state, hits, maxHits;
    private float lastSqrMag;
    public Entity target;
    Attack atk;
    GameObject horse, effect;

    enum ultimateState { CHARGE, SETUP, ATTACK, END }

    protected override void initComponents()
    {
        base.initComponents();
        charges = 0;
        hits = 0;
        state = 0;
        maxHits = 4; 
        chargesImages = instanceUI.GetComponentsInChildren<Image>();
    }

    public override void startUltimate()
    {
        base.startUltimate();
        effect = Instantiate(((TemplarAttacks)user.attacks).effects[3], user.transform, false);
        horse = Instantiate(((TemplarAttacks)user.attacks).effects[4], user.transform, false);
        if (user.facingDirection == Utilities.direction.Right) chargePosition = new Vector3(user.transform.position.x + 20, user.transform.position.y, user.transform.position.z);
        else chargePosition = new Vector3(user.transform.position.x - 20, user.transform.position.y, user.transform.position.z);
        direction = (chargePosition - user.transform.position).normalized * (user.movementSpeed* 2.5f);
        lastSqrMag = Mathf.Infinity;
    }

    public override void ultimateUpdate()
    {
        switch (state)
        {
            case 0: chargeState(); break;
            case 1: setupState(); break;
            case 2: attackState(); break;
        }     
    }

    private void attackState()
    {
        user.RigidBody.velocity = direction;
        float sqrMag = (chargePosition - user.transform.position).sqrMagnitude;
        if (sqrMag > lastSqrMag)
        {
            swapDirection();
            return;
        }
        lastSqrMag = sqrMag;
    }

    private void swapDirection()
    {
        if(user.facingDirection == Utilities.direction.Right)
        {
            direction = (leftPoint - user.transform.position).normalized * (user.movementSpeed * 3);
            chargePosition = leftPoint;
            user.movementX = -1;
            lastSqrMag = Mathf.Infinity;
            atk.resetHittedEntities();
        }
        else
        {
            direction = (rightPoint - user.transform.position).normalized * (user.movementSpeed * 3);
            chargePosition = rightPoint;
            user.movementX = 1;
            lastSqrMag = Mathf.Infinity;
            atk.resetHittedEntities();
        }
        user.checkFacingDirection();
    }

    private void setupState()
    {
        user.RigidBody.velocity = direction;
        float sqrMag = (topPoint - user.transform.position).sqrMagnitude;
        if (sqrMag > lastSqrMag)
        {
            startAttacks();
            return;
        }
        lastSqrMag = sqrMag;
    }

    private void startAttacks()
    {
        state = (int)ultimateState.ATTACK;
        
        if (user.facingDirection == Utilities.direction.Right)
        {
            rightPoint = new Vector3(user.transform.position.x + 12, user.transform.position.y, user.transform.position.z);
            leftPoint = new Vector3(user.transform.position.x - 3, user.transform.position.y, user.transform.position.z);
            direction = (rightPoint - user.transform.position).normalized * (user.movementSpeed * 3);
            user.transform.Rotate(new Vector3(0, 0, -10));
            target.transform.Rotate(new Vector3(0, 0, -10));
            chargePosition = rightPoint;
        }
        else {
            rightPoint = new Vector3(user.transform.position.x + 3, user.transform.position.y, user.transform.position.z);
            leftPoint = new Vector3(user.transform.position.x - 12, user.transform.position.y, user.transform.position.z);
            direction = (leftPoint - user.transform.position).normalized * (user.movementSpeed * 3);
            user.transform.Rotate(new Vector3(0, 0, 10));
            target.transform.Rotate(new Vector3(0, 0, 10));
            chargePosition = leftPoint;
        }
        lastSqrMag = Mathf.Infinity;
        target.transform.SetParent(null, true);
    }

    private void chargeState()
    {
        user.RigidBody.velocity = direction;
        float sqrMag = (chargePosition - user.transform.position).sqrMagnitude;
        if (sqrMag > lastSqrMag)
        {
            if (state == (int)ultimateState.CHARGE)
                endUltimate();
            return;
        }
        lastSqrMag = sqrMag;
    }

    void updateChargesUI()
    {
        int i;
        for (i = 0; i < charges; i++)
        {
            chargesImages[i].color = Color.white;
        }
        for(int x = i; x<3; x++)
        {
            chargesImages[x].color = Color.gray;
        }
    }

    public override void onBlock()
    {
        if (charges < 3) charges++;
        updateChargesUI();
    }

    public void decreaseCharges()
    {
        if (charges > 0) charges--;
        updateChargesUI();
    }

    public int getCharges()
    {
        return charges;
    }

    public override void endUltimate()
    {
        if (state != (int)ultimateState.SETUP) {
            base.endUltimate();
            Destroy(horse);
            Destroy(effect);
            user.RigidBody.velocity = new Vector2(0, 0);
            user.RigidBody.gravityScale = user.gravityAmount;
            if (user.isGrounded)
            {
                user.state = Utilities.state.Idle;
                user.Animator.Play("Ground_Movement");
            }
            else {
                user.state = Utilities.state.Air;
                user.Animator.Play("Falling");
            }
            state = (int)ultimateState.CHARGE;
        } 
    }

    /// <summary>
    /// Checks current charges and acts accordingly to current amount.
    /// </summary>
    /// <param name="attk"></param>
    public void chargesCheck(Attack attk)
    {
        switch (getCharges())
        {
            case 0: //0 charges normal attack.
                {
                    attk.damage = attk.initialDamage;
                    attk.stuns = false;
                }
                break;
            case 1: //1 charge, faster animation +1damage
                {
                    attk.user.Animator.speed = 2f;
                    user.attackEffect.GetComponent<Animator>().speed = 2f;
                    attk.damage = attk.initialDamage + 1;
                    decreaseCharges();
                    attk.stuns = false;
                }
                break;
            case 2: //faster animation, bigger effect +2damage
                {
                    attk.user.Animator.speed = 2f;
                    user.attackEffect.GetComponent<Animator>().speed = 2f;
                    user.attackEffect.transform.localScale = new Vector3(user.attackEffect.transform.localScale.x * 2, user.attackEffect.transform.localScale.y * 2, 1);
                    attk.damage = attk.initialDamage + 2;
                    decreaseCharges();
                    attk.stuns = false;
                }
                break;
            case 3://faster animation, bigger effect, stuns +3damage
                {
                    attk.user.Animator.speed = 2f;
                    user.attackEffect.GetComponent<Animator>().speed = 2f;
                    user.attackEffect.transform.localScale = new Vector3(user.attackEffect.transform.localScale.x * 2, user.attackEffect.transform.localScale.y * 2, 1);
                    attk.damage = attk.initialDamage + 3;
                    attk.stuns = true;
                    decreaseCharges();
                }
                break;
        }
    }

    public void ultimateHit()
    {
        hits++;
        if (hits == maxHits)
        {
            endUltimate();
            target.RigidBody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    internal void startSetup(Entity target, Attack atk)
    {
        this.target = target;
        target.transform.SetParent(user.transform, true);
        target.RigidBody.bodyType = RigidbodyType2D.Kinematic;
        target.RigidBody.velocity = new Vector2(0, 0);
        target.transform.localPosition = new Vector3(3.9f, -0.5f, 0);
        if (user.facingDirection == Utilities.direction.Right)
        {
            user.transform.Rotate(new Vector3(0, 0, 10));
            target.transform.Rotate(new Vector3(0, 0, 10));
            topPoint = new Vector3(user.transform.position.x + 12, user.transform.position.y + 10, user.transform.position.z);
        }
        else {
            user.transform.Rotate(new Vector3(0, 0, -10));
            target.transform.Rotate(new Vector3(0, 0, -10));
            topPoint = new Vector3(user.transform.position.x - 12, user.transform.position.y + 10, user.transform.position.z);
        }
        direction = (topPoint - user.transform.position).normalized * (user.movementSpeed * 2.5f);
        lastSqrMag = Mathf.Infinity;
        state = (int)ultimateState.SETUP;
        if (target.state == Utilities.state.Attacking) target.stopAttacking();
        target.state = Utilities.state.Hit;
        target.Animator.Play("Hit");
        this.atk = atk;
    }
}
