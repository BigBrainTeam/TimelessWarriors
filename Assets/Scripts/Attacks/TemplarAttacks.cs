using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplarAttacks : AttacksController {
    static public int dmg = 6;
    public GameObject[] effects;
    protected override void initializeComponents()
    {
        base.initializeComponents();
        user = GetComponent<Character>();  
        attacks = new Attack[10];
        attacks[0] = new BasicSideAttack(user, dmg, 6, false, SoundManager.instance.audioclips["swordSFX"]);
        attacks[1] = new BasicUpAttack(user, dmg-1, 5, false, SoundManager.instance.audioclips["swordSFX"]);
        attacks[2] = new BasicMovingAttack(user, dmg, 10);
        attacks[3] = new BasicSideAttack(user, dmg, 8, true, SoundManager.instance.audioclips["swordSFX"]);
        attacks[4] = new BasicUpAttack(user, dmg-1, 8, true, SoundManager.instance.audioclips["swordSFX"]);
        attacks[5] = new BasicDownAttack(user, dmg-1, 9, SoundManager.instance.audioclips["swordSFX"]);
        attacks[6] = new TemplarSpecialSide(user, effects[0]);
        attacks[7] = new TemplarSpecialUp(user, effects[1]);
        attacks[8] = new TemplarSpecialDown(user, effects[2]);
        attacks[9] = new TemplarFinal(user);
    }

    //Special external functions for animation
    public void specialUpImpulse()
    {
        ((TemplarSpecialUp)attacks[7]).up();   
    }
    public void specialUpDrop()
    {
        ((TemplarSpecialUp)attacks[7]).down();
    }
    public void specialUpTop()
    {
        ((TemplarSpecialUp)attacks[7]).top();
    }
    public void specialDownExplosion()
    {
        ((TemplarSpecialDown)attacks[8]).explosion();
    }
    public void checkDrop()
    {
        if(user is Player)
        {
            if (Input.GetButton(((Player)user).controls.SpecialAttack)) user.Animator.SetBool("drop", true);
        }      
    }
    public void destroyEffect()
    {
        GameObject.Destroy(user.attackEffect);
    }
}

public class TemplarSpecialSide : Attack {

    public TemplarSpecialSide(Entity user, GameObject eff) : base(user)
    {
        effect = eff;
        name = "SpecialSide";
        damage = TemplarAttacks.dmg+1;
        knockBack = 10f;
        animationName = "SpecialSide";
        type = "Side";
        stops = false;
        stunTime = 25f;
        initialDamage = damage;
    }
    public override void doAttack()
    {
        GameObject go = GameObject.Instantiate(effect, user.transform, false);
        Templar tmp = (Templar)((Character)user).speciality;
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, go.GetComponent<Collider2D>());
        user.attackEffect = go;
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        tmp.chargesCheck(this);

        base.doAttack();
    } 
}
public class TemplarSpecialUp : Attack {
    public TemplarSpecialUp(Entity user, GameObject eff) : base(user)
    {
        effect = eff;
        name = "SpecialUp";
        damage = TemplarAttacks.dmg;
        knockBack = 10f;
        animationName = "Start";
        stops = true;
    }
    public void up()
    {
        user.Animator.SetBool("drop", false);
        ((Character)user).canSpecialUp = false;
        user.RigidBody.gravityScale = user.gravityAmount;
        type = "Up";
        user.RigidBody.velocity = new Vector2(0, 0);
        Vector2 f = new Vector2(0f, 30f);
        user.RigidBody.AddForce(f, ForceMode2D.Impulse);
        user.attackEffect = GameObject.Instantiate(effect, user.transform, false);
        user.attackEffect.transform.Rotate(0, 0, 180);
    }
    public void down()
    {
        resetHittedEntities();
        type = "Down";
        user.RigidBody.velocity = new Vector2(0, 0);
        Vector2 f = new Vector2(0f, -25f);
        user.RigidBody.AddForce(f, ForceMode2D.Impulse);
        user.attackEffect = GameObject.Instantiate(effect, user.transform, false);
    }

    public void top()
    {
        resetHittedEntities();
        user.RigidBody.velocity /= 2;
        GameObject.Destroy(user.attackEffect);
    }

}

public class TemplarSpecialDown : Attack{

    Templar tmp;

    public TemplarSpecialDown(Entity user, GameObject eff) : base(user)
    {
        name = "SpecialDown";
        damage = TemplarAttacks.dmg;
        knockBack = 7f;
        animationName = "SpecialDown";
        type = "Up";
        stops = false;
        effect = eff;
        stunTime = 25f;
        tmp = (Templar)((Character)user).speciality;
        initialDamage = damage;
    }

    public void explosion()
    {
        user.attackEffect.SetActive(true);
    }

    public override void doAttack()
    {
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        user.attackEffect = GameObject.Instantiate(effect, user.transform, false);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, user.attackEffect.GetComponent<Collider2D>());
        tmp.chargesCheck(this);
        user.attackEffect.SetActive(false);
        base.doAttack();      
    }
}

public class TemplarFinal : Attack {
    Character cUser;
    Templar temp;

    public TemplarFinal(Entity user) : base(user)
    {
        name = "Final";
        damage = TemplarAttacks.dmg + 4f;
        knockBack = 10f;
        animationName = "Ultimate";
        stops = true;
        type = "Side";
        cUser = (Character)user;
        temp = (Templar)cUser.speciality;

    }
    public override void doAttack()
    {
        temp.startUltimate();
        GameManager.Instance.currentFight.onUltimateUse(((Character)user).player);
        cUser.setEnergy(0);
        base.doAttack();
    }
    public override void onHit(Entity target)
    {
        if (temp.state == 0)
        {
            temp.startSetup(target, this);
            hittedEntities.Add(target);
        }
        else
        {
            if (target == temp.target)
            {
                if (canBeHit(target))
                {
                    temp.ultimateHit();
                    if (temp.hits < temp.maxHits)
                    {
                        target.receiveDamage(damage);
                        target.hitEffect();
                    }
                    else
                    {
                        temp.hits = 0;
                        base.onHit(target);
                    }
                }
            }
            else
            {
                base.onHit(target);
            }
        }
    }
}
