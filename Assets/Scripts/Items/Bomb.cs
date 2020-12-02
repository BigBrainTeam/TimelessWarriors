using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : ThrowableItem {

    float pickableVelocity = 0f;
    public GameObject bombExplosion;
    public LayerMask mylayer;
    private Collider2D[] targetsColliders;

    public override void itemAction()
    {
        Collider2D[] targetsColliders = Physics2D.OverlapCircleAll(transform.position, 2, mylayer);
        isUsed = false;
        GameObject bombExplosionID = Instantiate(bombExplosion, transform.position, transform.rotation);
        Destroy(bombExplosionID, 50f * Time.deltaTime);
        foreach (Collider2D c in targetsColliders)
        {
            Entity currentEntity = c.gameObject.GetComponentInParent<Entity>();
            currentEntity.State = Utilities.state.Hit;
            StartCoroutine(currentEntity.receiveHit(damage, knockBack, this));
        }
        GameManager.Instance.currentFight.onObjectDestroyed(this.gameObject);
        Destroy(this.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(isUsed && other.GetComponentInParent<Entity>()) itemAction();
    }

    protected override void initializeComponents()
    {
        base.initializeComponents();
        damage = 15;
        velocity = 30;
        knockBack = 5;
        itemType = type.explosive;
    }

    new void Update()
    {
        base.Update();
        checkVelocityToPick();
        if (isUsed) anim.SetBool("isUsing", true);
        else anim.SetBool("isUsing", false);
        if (isPicked) anim.SetBool("isStatic", true);
        else anim.SetBool("isStatic", false);
    }

    protected void checkVelocityToPick()
    {
        if (rbd.velocity.magnitude < pickableVelocity)
        {
            isUsed = false;
        }
    }
}
