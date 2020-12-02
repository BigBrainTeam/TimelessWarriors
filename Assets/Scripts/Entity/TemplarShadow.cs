using System;
using UnityEngine;

public  class TemplarShadow: MonoBehaviour
{
    Animator anim;
    SpriteRenderer sprite;
    float speed;
    bool goingUp, goingSide;
    public Character blackTemplar;

    Attack[] shadowAttacks;
    int currentAttack;
    Collider2D hurtbox;

    public void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        speed = 30f;
        goingUp = false;
        blackTemplar = GetComponentInParent<Character>();
        shadowAttacks = new Attack[8];
        shadowAttacks[0] = new BasicSideAttack(blackTemplar, BlackTemplarAttacks.dmg + 1, 6, false);
        shadowAttacks[1] = new BasicUpAttack(blackTemplar, BlackTemplarAttacks.dmg, 5, false);
        shadowAttacks[2] = new BasicMovingAttack(blackTemplar, BlackTemplarAttacks.dmg, 10);
        shadowAttacks[3] = new BasicSideAttack(blackTemplar, BlackTemplarAttacks.dmg + 1, 8, true);
        shadowAttacks[4] = new BasicUpAttack(blackTemplar, BlackTemplarAttacks.dmg, 8, true);
        shadowAttacks[5] = new BasicDownAttack(blackTemplar, BlackTemplarAttacks.dmg, 9);
        shadowAttacks[6] = new BasicSideAttack(blackTemplar, BlackTemplarAttacks.dmg + 3, 9, true);
        shadowAttacks[7] = new BasicUpAttack(blackTemplar, BlackTemplarAttacks.dmg*2+1, 9, true);
        Physics2D.IgnoreCollision(blackTemplar.transform.GetChild(2).GetComponent<Collider2D>(), GetComponent<Collider2D>());
        if (GameManager.Instance.currentFight is TeamFight)
        {
            Entity teammate = ((TeamFight)GameManager.Instance.currentFight).getTeammate(blackTemplar.player);
            Physics2D.IgnoreCollision(teammate.transform.GetChild(2).GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
        hurtbox = transform.GetChild(0).GetComponent<Collider2D>();
    }
    public void Update()
    {
        if (goingUp) flyUp();
        if (goingSide) boostSide();
    }
    public void checkDirection()
    {
        if(transform.parent == null)
        {
            if (blackTemplar.facingDirection == Utilities.direction.Right) transform.localScale = new Vector3(1.1f, transform.localScale.y, transform.localScale.z);
            else transform.localScale = new Vector3(-1.1f, transform.localScale.y, transform.localScale.z);
        }
    }
    private void boostSide()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        if (blackTemplar.facingDirection == Utilities.direction.Right) {
            speed++;
            if (speed > 0) speed = 0;
        }
        else
        {
            speed--;
            if (speed < 0) speed = 0;
        }
    }

    public void doAttack(string animation)
    {
        checkDirection();
        sprite.enabled = true;
        anim.Play(animation);
        switch (animation)
        {
            case "BasicFloorSide": currentAttack = 0; break;
            case "BasicFloorUp": currentAttack = 1; break;
            case "BasicAirSide": currentAttack = 3; break;
            case "BasicAirUp": currentAttack = 4; break;
            case "BasicAirDown": currentAttack = 5; break;
            case "SpecialSide": currentAttack = 6; break;
            case "SpecialUp": currentAttack = 7; break;
        }
    }

    public void stopAttack()
    {
        shadowAttacks[currentAttack].resetHittedEntities();
        resetComps();
    }

    public void drop()
    {
        hurtbox.enabled = true;
        transform.SetParent(null, true);
        sprite.enabled = true;
        anim.Play("SpecialDown");
    }

    public void teleport()
    {
        goingUp = false;
        speed = 30;
        sprite.enabled = false;
        anim.Play("Idle");
    }
    public void specialSide()
    {
        goingSide = true;
        sprite.enabled = true;
        anim.Play("SpecialSide");
        transform.SetParent(null, true);
        if (blackTemplar.facingDirection == Utilities.direction.Right)
        {
            speed = -25;
            transform.position = new Vector2(blackTemplar.transform.position.x + 12, transform.position.y);
            transform.localScale = new Vector3(-1.1f, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            speed = 25;
            transform.position = new Vector2(blackTemplar.transform.position.x - 12, transform.position.y);
            transform.localScale = new Vector3(1.1f, transform.localScale.y, transform.localScale.z);
        }
    }
    internal void flyUp()
    {
        transform.position = new Vector3(transform.position.x , transform.position.y + speed * Time.deltaTime, transform.position.z);
        speed -= 1;
    }

    public void startJump()
    {
        goingUp = true;
        transform.SetParent(null, true);
    }

    internal void resetComps()
    {
        checkDirection();
        speed = 30;
        hurtbox.enabled = false;
        anim.Play("Idle");
        sprite.enabled = false;
        goingUp = false;
        transform.SetParent(blackTemplar.transform, true);
        transform.localPosition = ((BlackTemplar)blackTemplar.speciality).shadowPos;
        ((BlackTemplar)blackTemplar.speciality).hasShadow = true;
    }
    public void stopSide()
    {
        goingSide = false;
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>())
        {
            Entity target = collision.GetComponentInParent<Entity>();
            if (target.state != Utilities.state.Blocking) shadowAttacks[currentAttack].onHit(target);
            else target.onBlock();
        }
    }
}