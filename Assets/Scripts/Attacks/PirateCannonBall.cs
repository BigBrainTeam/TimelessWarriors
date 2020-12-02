using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateCannonBall : Projectile {

    public GameObject cannon;
    List<Entity> draggingTargets;
    [SerializeField]
    int maxCannons;
    int currentCannons = 0;
    public bool spawner;

    override protected void Start()
    {
        base.Start();
        draggingTargets = new List<Entity>();
    }

    new private void Update()
    {
        setThemBelow();
    }
    /// <summary>
    /// Sets the entities being dragged below the CannonBall
    /// </summary>
    private void setThemBelow()
    {
        if (draggingTargets.Count > 0)
        {
            foreach(Entity ent in draggingTargets) ent.transform.localPosition = new Vector3(0, -0.6f, 0);
        }
    }

    new private void OnBecameInvisible()
    {
        if (spawner)
        {
            Destroy(cannon);
            rbd.velocity = new Vector2(0, 0);
            this.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine(cannonBallsRain());
        }
    }
    /// <summary>
    /// Spawns cannonballs.
    /// </summary>
    /// <returns></returns>
    private IEnumerator cannonBallsRain()
    {
        float y = GameObject.Find("Up").transform.position.y - 5;
        while (currentCannons < maxCannons)
        {
            Vector3 spawnPosition = new Vector3(GameManager.Instance.currentFight.itemSpawner.getRandomX(), y, 0);
            GameObject newCannonBall = Instantiate(this.gameObject, spawnPosition, Quaternion.identity);
            newCannonBall.GetComponent<Collider2D>().enabled = true;
            newCannonBall.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            newCannonBall.GetComponent<SpriteRenderer>().enabled = true;
            PirateCannonBall proj = newCannonBall.GetComponent<PirateCannonBall>();
            proj.direction = Utilities.direction.Down;
            proj.attack = attack;
            proj.spawner = false;
            Physics2D.IgnoreCollision(newCannonBall.GetComponent<Collider2D>(), user.HurtBox, true);
            if (GameManager.Instance.currentFight is TeamFight)
            {
                Entity teammate = ((TeamFight)GameManager.Instance.currentFight).getTeammate(((Character)user).player);
                Physics2D.IgnoreCollision(teammate.transform.GetChild(2).GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
            currentCannons++;
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(this.gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Entity>())
        {
            Entity ent = collision.GetComponentInParent<Entity>();
            if (canBeHit(ent))
            {
                if (ent.state != Utilities.state.Blocking)
                {
                    startCannonDrag(ent);
                }
                else
                {
                    ent.onBlock();
                    Destroy(this.gameObject);
                }
            }          
        }
        else
        {
            if(rbd.velocity.y < 0) explode();
        }
    }

    private bool canBeHit(Entity ent)
    {
        foreach(Entity e in draggingTargets)
        {
            if (e == ent) return false;
        }
        return true;
    }
    /// <summary>
    /// Explodes the CannonBall, knocking each entity being dragged.
    /// </summary>
    private void explode()
    {
        Instantiate(hitEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        //particle effect
        foreach (Entity e in draggingTargets)
        {
            e.RigidBody.bodyType = RigidbodyType2D.Dynamic;
            e.transform.parent = null;
            e.transform.localEulerAngles = new Vector3(0, 0, 0);
            attack.onHit(e);
        }
        attack.resetHittedEntities();
        Destroy(this.gameObject);
    }
    /// <summary>
    /// Sets up the hitted Entity for dragging.
    /// </summary>
    /// <param name="ent"></param>
    private void startCannonDrag(Entity ent)
    {
        ent.receiveDamage(attack.damage);
        ent.hitEffect();
        ent.Animator.Play("Hit");
        if (ent.state == Utilities.state.Attacking) ent.stopAttacking();
        ent.state = Utilities.state.Hit;

        ent.transform.SetParent(this.transform, true);
        if (ent.facingDirection == Utilities.direction.Right) ent.transform.localEulerAngles = new Vector3(0, 0, 90);
        else ent.transform.localEulerAngles = new Vector3(0, 0, -90);
        ent.RigidBody.bodyType = RigidbodyType2D.Kinematic;
        ent.RigidBody.velocity = new Vector2(0, 0);
        ent.transform.localPosition = new Vector3(0, -0.6f, 0);
        draggingTargets.Add(ent);
    }
}
