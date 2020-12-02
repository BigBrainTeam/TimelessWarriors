using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateAttacks : AttacksController
{
    static public int dmg = 5;
    public GameObject[] effects;
    public GameObject[] spawns;

    protected override void initializeComponents()
    {
        base.initializeComponents();
        user = GetComponent<Character>();
        attacks = new Attack[10];
        attacks[0] = new BasicSideAttack(user, dmg, 6, false, SoundManager.instance.audioclips["swordSFX"]);
        attacks[1] = new PirateBasicFloorUp(user, dmg, 5, false, SoundManager.instance.audioclips["shotSFX"]);
        attacks[2] = new BasicMovingAttack(user, dmg, 10);
        attacks[3] = new BasicSideAttack(user, dmg, 8, true, SoundManager.instance.audioclips["swordSFX"]);
        attacks[4] = new PirateBasicAirUp(user, dmg - 1, 8, true, SoundManager.instance.audioclips["shotSFX"]);
        attacks[5] = new PirateBasicAirDown(user, dmg - 1, 9, SoundManager.instance.audioclips["shotSFX"]);
        attacks[6] = new PirateSpecialSide(user);
        attacks[7] = new PirateSpecialUp(user);
        attacks[8] = new PirateSpecialDown(user);
        attacks[9] = new PirateFinal(user, spawns[1]);
    }
    public void pistolShot()
    {
        Instantiate(effects[0], user.transform.GetChild(1).transform.position, Quaternion.identity);
    }
    public void spawnCanonBall()
    {
        GameObject go = Instantiate(spawns[0], user.transform, false);
        go.transform.parent = null;
        Collider2D hitbox = go.GetComponent<Collider2D>();
        Projectile ball = go.GetComponent<Projectile>();
        ball.attack = attacks[6];
        Physics2D.IgnoreCollision(user.HurtBox, hitbox);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, hitbox);
        ball.user = user;
        ball.direction = user.facingDirection;
    }
    public void specialDown()
    {
        user.attackEffect = Instantiate(effects[1], user.transform, false);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, user.attackEffect.GetComponent<BoxCollider2D>());
    }
    public void spawnWater()
    {
        GameObject go = Instantiate(spawns[2], user.transform, false);
        go.transform.parent = null;
        Collider2D hitbox = go.GetComponent<Collider2D>();
        PirateWaterUp waterUp = go.GetComponent<PirateWaterUp>();
        user.attackEffect = go;
        Physics2D.IgnoreCollision(user.HurtBox, hitbox);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, hitbox);
        waterUp.user = (Character)user;
        waterUp.atk = attacks[7];
    }
}

public class PirateBasicFloorUp : BasicUpAttack
{
    Pirate spec;
    public PirateBasicFloorUp(Entity user, float dmg, float kback, bool air, AudioClip sound) : base(user,dmg,kback,air,sound)
    {
        spec = (Pirate)user.speciality;
    }
    public override void onHit(Entity target)
    {
        base.onHit(target);
        spec.pistolHit(target);
    }
}

public class PirateBasicAirUp : BasicUpAttack
{
    Pirate spec;
    public PirateBasicAirUp(Entity user, float dmg, float kback, bool air, AudioClip sound) : base(user, dmg, kback, air, sound)
    {
        spec = (Pirate)user.speciality;
    }
    public override void onHit(Entity target)
    {
        base.onHit(target);
        spec.pistolHit(target);
    }
}

public class PirateBasicAirDown : BasicDownAttack
{
    Pirate spec;
    public PirateBasicAirDown(Entity user, float dmg, float kback, AudioClip sound) : base(user, dmg, kback, sound)
    {
        spec = (Pirate)user.speciality;
    }
    public override void onHit(Entity target)
    {
        base.onHit(target);
        spec.pistolHit(target);
    }
}


public class PirateSpecialSide : Attack
{
    Projectile ball;
    public PirateSpecialSide(Entity user) : base(user)
    {
        name = "SpecialSide";
        damage = PirateAttacks.dmg / 2;
        knockBack = 0.5f;
        animationName = "SpecialSide";
        type = "Side";
        stops = false;
    }
    public override void doAttack()
    {
        user.RigidBody.velocity = new Vector2(0, user.RigidBody.velocity.y);
        base.doAttack();
    }
    public override void onHit(Entity target, GameObject projectile)
    {
        ball = projectile.GetComponent<Projectile>();

        base.onHit(target);
    }
    public override void knockBackEntity(Entity target, float totalKnockBack)
    {
        sideKnockBackEntity(target, totalKnockBack, ball.direction);
    }
}
public class PirateSpecialUp : Attack
{
    Pirate spec;
    public PirateSpecialUp(Entity user) : base(user)
    {
        name = "SpecialUp";
        damage = PirateAttacks.dmg + 1;
        knockBack = 10f;
        animationName = "SpecialUp_Start";
        stops = true;
        type = "Up";
        spec = (Pirate)user.speciality;
    }
    public override void doAttack()
    {
        ((Character)user).canSpecialUp = false;
        base.doAttack();
    }
    public override void onHit(Entity target)
    {
        base.onHit(target);
        spec.waterHit(target);
    }
}

public class PirateSpecialDown : Attack
{
    Pirate spec;
    Vector3 spawnPosition;

    public PirateSpecialDown(Entity user) : base(user)
    {
        name = "SpecialDown";
        damage = PirateAttacks.dmg + 2;
        knockBack = 10f;
        animationName = "SpecialDown";
        type = "Diagonal";
        stops = false;
        spec = (Pirate)user.speciality;
    }

    public override void doAttack()
    {
        base.doAttack();
        spawnPosition = user.transform.position;
    }
    public override void onHit(Entity target)
    {
        base.onHit(target);
        spec.waterHit(target);
    }
}

public class PirateFinal : Attack
{
    Pirate pirat;
    GameObject cannon;
    Character cUser;
    public PirateFinal(Entity user, GameObject spawn) : base(user)
    {
        name = "Final";
        damage = PirateAttacks.dmg + 5;
        knockBack = 5f;
        animationName = "Ultimate_Start";
        type = "Up";
        stops = false;
        cannon = spawn;
        cUser = (Character)user;
    }
    public override void doAttack()
    {
        GameObject instance = GameObject.Instantiate(cannon, user.transform, false);
        instance.transform.parent = null;
        PirateCannon cannonBall;
        cannonBall = instance.GetComponent<PirateCannon>();
        cannonBall.user = (Character)user;
        cannonBall.atk = this;
        user.state = Utilities.state.Idle;
        cUser.setEnergy(0);
        user.attacks.currentAttack = Utilities.attackType.None;
        user.HurtBox.enabled = true;
    }
}
