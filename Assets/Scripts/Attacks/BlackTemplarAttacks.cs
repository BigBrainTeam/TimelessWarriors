using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for Black Templar Attacks.
/// </summary>
public class BlackTemplarAttacks : AttacksController
{
    static public float dmg = 1.5f;
    public GameObject[] effects;

    protected override void initializeComponents()
    {
        base.initializeComponents();
        user = GetComponent<Character>();
        attacks = new Attack[10];
        attacks[0] = new BlackTemplarBasicSideAttack(user, dmg + 1, 6, false, SoundManager.instance.audioclips["swordSFX"]);
        attacks[1] = new BlackTemplarBasicUpAttack(user, dmg, 5, false, SoundManager.instance.audioclips["swordSFX"]);
        attacks[2] = new BasicMovingAttack(user, dmg, 10);
        attacks[3] = new BlackTemplarBasicSideAttack(user, dmg + 1, 8, true, SoundManager.instance.audioclips["swordSFX"]);
        attacks[4] = new BlackTemplarBasicUpAttack(user, dmg, 8, true, SoundManager.instance.audioclips["swordSFX"]);
        attacks[5] = new BlackTemplarBasicDownAttack(user, dmg, 9, SoundManager.instance.audioclips["swordSFX"]);
        attacks[6] = new BlackTemplarSpecialSide(user);
        attacks[7] = new BlackTemplarSpecialUp(user);
        attacks[8] = new BlackTemplarSpecialDown(user);
        attacks[9] = new BlackTemplarFinal(user, effects[0]);
    }

    public void callShadowDeploy()
    {
        ((BlackTemplarSpecialDown)attacks[8]).shadowCall();
    }
    public void stopBoost()
    {
        user.RigidBody.velocity = new Vector2(0, 0);
    }
    public void callShadowJump()
    {
        ((BlackTemplarSpecialUp)attacks[7]).shadowCall();
    }
    public void specialUpTeleport()
    {
        ((BlackTemplarSpecialUp)attacks[7]).specUpTp();
    }
}




public class BlackTemplarBasicSideAttack : BasicSideAttack
{
    BlackTemplar spec;
    public BlackTemplarBasicSideAttack(Entity user, float damage, float knockBack, bool air, AudioClip sound) : base(user, damage, knockBack, air, sound)
    {
        spec = (BlackTemplar)user.speciality;
        initialDamage = damage;
    }
    public override void doAttack()
    {
        spec.shadowAttack(this);
        base.doAttack();
    }
}
public class BlackTemplarBasicUpAttack : BasicUpAttack
{
    BlackTemplar spec;
    public BlackTemplarBasicUpAttack(Entity user, float damage, float knockBack, bool air, AudioClip sound) : base(user, damage, knockBack, air, sound)
    {
        spec = (BlackTemplar)user.speciality;
        initialDamage = damage;
    }
    public override void doAttack()
    {
        spec.shadowAttack(this);
        base.doAttack();
    }
}

public class BlackTemplarBasicDownAttack : BasicDownAttack
{
    BlackTemplar spec;
    public BlackTemplarBasicDownAttack(Entity user, float damage, float knockBack, AudioClip sound) : base(user, damage, knockBack, sound)
    {
        spec = (BlackTemplar)user.speciality;
        initialDamage = damage;
    }
    public override void doAttack()
    {
        spec.shadowAttack(this);
        base.doAttack();
    }
}

public class BlackTemplarSpecialSide : Attack
{
    BlackTemplar bTemplar;
    public BlackTemplarSpecialSide(Entity user) : base(user)
    {
        name = "SpecialSide";
        damage = BlackTemplarAttacks.dmg*2 + 1;
        knockBack = 0.5f;
        animationName = "SpecialSide";
        bTemplar = (BlackTemplar)user.speciality;
        type = "Side";
        stops = false;
    }
    public override void doAttack()
    {
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        if(user.facingDirection == Utilities.direction.Right) user.RigidBody.AddForce(new Vector2(15, 0), ForceMode2D.Impulse);
        else user.RigidBody.AddForce(new Vector2(-15, 0), ForceMode2D.Impulse); ;
        bTemplar.specialSide();
        base.doAttack();
    }
}
public class BlackTemplarSpecialUp : Attack
{
    BlackTemplar bTemplar;
    public BlackTemplarSpecialUp(Entity user) : base(user)
    {
        name = "SpecialUp";
        damage = BlackTemplarAttacks.dmg + 3;
        knockBack = 10f;
        animationName = "SpecialUp";
        stops = false;
        bTemplar = (BlackTemplar)user.speciality;
        type = "Diagonal";
    }
    public override void doAttack()
    {
        ((Character)user).canSpecialUp = false;
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        base.doAttack();
    }

    public void shadowCall()
    {
        bTemplar.specialUp(this);
    }
    public void specUpTp()
    {
        bTemplar.specialUpEnd();
    }
}

public class BlackTemplarSpecialDown : Attack
{
    BlackTemplar bTemplar;
    public BlackTemplarSpecialDown(Entity user) : base(user)
    {
        name = "SpecialDown";
        animationName = "SpecialDown";
        type = "Down";
        stops = true;
        bTemplar = (BlackTemplar)user.speciality;
    }

    public override void doAttack()
    {
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        bTemplar.specialUp(this);
        base.doAttack();
    }
    public void shadowCall()
    {
        bTemplar.specialDown();
    }
}

public class BlackTemplarFinal : Attack
{
    GameObject death;
    Character cUser;
    public BlackTemplarFinal(Entity user, GameObject effect) : base(user)
    {
        name = "Final";
        damage = BlackTemplarAttacks.dmg + 20;
        knockBack = 5f;
        animationName = "Ultimate_Start";
        type = "Side";
        stops = true;
        death = effect;
        cUser = (Character)user;
    }
    public override void doAttack()
    {
        cUser.HurtBox.enabled = false;
        user.RigidBody.velocity = new Vector2(0, 0);
        user.RigidBody.gravityScale = 0;
        GameObject deathInstance = GameObject.Instantiate(death, user.transform, false);
        ShadowUltimate shadowUlt = deathInstance.GetComponent<ShadowUltimate>();
        shadowUlt.user = cUser;
        Collider2D hitbox = deathInstance.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(user.transform.GetChild(2).GetComponent<Collider2D>(), hitbox);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(cUser.player, hitbox);
        cUser.setEnergy(0);
        cUser.Sprite.enabled = false;
        cUser.HurtBox.enabled = false;
    }
}