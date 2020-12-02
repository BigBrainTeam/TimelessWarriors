using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : ThrowableItem {

    bool rotator;
    protected override void initializeComponents()
    {
        base.initializeComponents();
        damage = 10;
        velocity = 50;
        knockBack = 5;
        itemType = type.bullet;
    }

    public override void itemAction(Collider2D c)
    {
        Entity target = c.GetComponentInParent<Entity>();
        isUsed = false;
        target.State = Utilities.state.Hit;
        StartCoroutine(target.receiveHit(damage, knockBack, this));
        GameManager.Instance.currentFight.onObjectDestroyed(this.gameObject);
        Destroy(this.gameObject);

    }

    public override void onThrow()
    {
        rotator = true;
        base.onThrow();
        rbd.gravityScale = 0f;
        rbd.constraints = RigidbodyConstraints2D.None;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (isUsed && c.GetComponentInParent<Entity>())
        {
            rbd.constraints = RigidbodyConstraints2D.FreezeAll;
            StopCoroutine("rotateOverTime");
            itemAction(c);
        }
        
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (isUsed)
        {
            rbd.constraints = RigidbodyConstraints2D.FreezeAll;
            StopCoroutine("rotateOverTime");
            transform.eulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            isUsed = false;
        }
    }

    protected override void Update()
    {
        base.Update();
        if(rotator) transform.Rotate(new Vector3(0f, 0f, 60f));
    }
}

