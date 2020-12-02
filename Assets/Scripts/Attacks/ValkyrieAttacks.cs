using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValkyrieAttacks : AttacksController {
    static public int dmg = 4;
    public GameObject[] effects;
    public GameObject[] spawns;

    protected override void initializeComponents()
    {
        base.initializeComponents();
        user = GetComponent<Character>();  
        attacks = new Attack[10];
        attacks[0] = new BasicSideAttack(user, dmg+1, 6, false, SoundManager.instance.audioclips["daggerSFX"]);
        attacks[1] = new BasicUpAttack(user, dmg+1, 5, false, SoundManager.instance.audioclips["daggerSFX"]);
        attacks[2] = new BasicMovingAttack(user, dmg+1, 10);
        attacks[3] = new BasicSideAttack(user, dmg+1, 8, true, SoundManager.instance.audioclips["daggerSFX"]);
        attacks[4] = new BasicUpAttack(user, dmg, 8, true, SoundManager.instance.audioclips["daggerSFX"]);
        attacks[5] = new BasicDownAttack(user, dmg, 9, SoundManager.instance.audioclips["daggerSFX"]);
        attacks[6] = new ValkyrieSpecialSide(user);
        attacks[7] = new ValkyrieSpecialUp(user);
        attacks[8] = new ValkyrieSpecialDown(user);
        attacks[9] = new ValkyrieFinal(user, effects[0]);
    }

    public void spawnSpear()
    {
        GameObject go = Instantiate(spawns[0], user.transform.GetChild(1).transform.position, Quaternion.identity);
        Collider2D hitbox = go.GetComponent<Collider2D>();
        Projectile spear = go.GetComponent<Projectile>();
        spear.attack = attacks[6];
        Physics2D.IgnoreCollision(user.HurtBox, hitbox);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, hitbox);
        spear.user = user;
        spear.direction = user.facingDirection;
    }
    public void specialUpJump()
    {
        user.RigidBody.gravityScale = user.gravityAmount;
        int dir = 1;
        if (user.facingDirection == Utilities.direction.Left) dir = -1;
        user.RigidBody.AddForce(new Vector2(15*dir, 25), ForceMode2D.Impulse);
    }
    public void ultimateAttackEffect()
    {
        ((ValkyrieFinal)attacks[9]).spawnEffect();
    }
    public void specialUpEffect()
    {
        user.attackEffect = Instantiate(effects[1], user.transform, false);
        user.attackEffect.transform.parent = null;
    }
    public void stopBoost()
    {
        user.RigidBody.velocity = user.RigidBody.velocity / 2;
    }
    public void spawnThunder()
    {
        Vector3 spawnPos = new Vector3(user.transform.position.x - 1.5f, user.transform.position.y + 5.5f, 0);
        GameObject go = Instantiate(spawns[1], spawnPos,Quaternion.Euler(0,0,-90));
        Collider2D hitbox = go.GetComponent<Collider2D>();
        ValkyrieThunder thunder = go.GetComponent<ValkyrieThunder>();
        Physics2D.IgnoreCollision(user.HurtBox, hitbox);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, hitbox);
        thunder.user = (Character)user;
    }
}

public class ValkyrieSpecialSide : Attack {

    Projectile spear;
    public ValkyrieSpecialSide(Entity user) : base(user)
    {
        name = "SpecialSide";
        damage = ValkyrieAttacks.dmg/2;
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
        spear = projectile.GetComponent<Projectile>();
        base.onHit(target);
    }
    public override void knockBackEntity(Entity target, float totalKnockBack)
    {
        sideKnockBackEntity(target, totalKnockBack, spear.direction);
    }
}
public class ValkyrieSpecialUp : Attack {
    public ValkyrieSpecialUp(Entity user) : base(user)
    {
        name = "SpecialUp";
        damage = ValkyrieAttacks.dmg+2;
        knockBack = 10f;
        animationName = "Start";
        stops = true;
        type = "Diagonal";
    }
    public override void doAttack()
    {
        ((Character)user).canSpecialUp = false;
        base.doAttack();
    }
}

public class ValkyrieSpecialDown : Attack{

    Vector3 spawnPosition;

    public ValkyrieSpecialDown(Entity user) : base(user)
    {
        name = "SpecialDown";
        damage = ValkyrieAttacks.dmg+3;
        knockBack = 10f;
        animationName = "SpecialDown";
        type = "Down";
        stops = true;
    }

    public override void doAttack()
    {
        base.doAttack();
        spawnPosition = user.transform.position;
    }
}

public class ValkyrieFinal : Attack {
    Valkyrie valk;
    Character cUser;
    public ValkyrieFinal(Entity user, GameObject eff) : base(user)
    {
        name = "Final";
        damage = ValkyrieAttacks.dmg+6;
        knockBack = 5f;
        animationName = "Ultimate_Start";
        type = "Side";
        stops = true;
        effect = eff;
        cUser = (Character)user;
        valk = (Valkyrie)cUser.speciality;
    }
    public override void doAttack()
    {
        if(valk.state == 0)
        {
            animationName = "Ultimate_Start";
            GameManager.Instance.currentFight.onUltimateUse(cUser.player);
            cUser.setEnergy(0);
            valk.startUltimate();
            base.doAttack();
            animationName = "Ultimate_Attack";
        }
        else
        {
            base.doAttack();
        }       
    }
    public void spawnEffect()
    {
        user.attackEffect = GameObject.Instantiate(effect, user.transform, false);
        if (GameManager.Instance.currentFight is TeamFight) ((TeamFight)GameManager.Instance.currentFight).ignoreTeammate(((Character)user).player, user.attackEffect.GetComponent<Collider2D>());
    }
}
