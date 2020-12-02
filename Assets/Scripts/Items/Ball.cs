using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : NoPickableItem {

    public LayerMask playerLayer;

    // Use this for initialization
    protected override void initializeComponents()
    {
        base.initializeComponents();
        velocity = 40;
        damage = 5;
        knockBack = 5;
        itemType = type.explosive;
    }

    new void Update()
    {
        base.Update();
    }

    public void addForceByAttack(Entity user, float knockBack)
    {
        if (user.getIsGrounded())
        {
            if (user.FacingDirection == Utilities.direction.Right || user.FacingDirection == Utilities.direction.UpRight) rbd.AddForce(new Vector2(knockBack, knockBack * 1.5f), ForceMode2D.Impulse);
            else if (user.FacingDirection == Utilities.direction.Left || user.FacingDirection == Utilities.direction.UpLeft) rbd.AddForce(new Vector2(-knockBack, knockBack * 1.5f), ForceMode2D.Impulse);
            else if (user.FacingDirection == Utilities.direction.Up) rbd.AddForce(new Vector2(0, knockBack * 1.5f), ForceMode2D.Impulse);
            else if (user.FacingDirection == Utilities.direction.Down) rbd.AddForce(new Vector2(0, -knockBack * 1.5f), ForceMode2D.Impulse);
            else if (user.FacingDirection == Utilities.direction.DownLeft) rbd.AddForce(new Vector2(-knockBack, -knockBack), ForceMode2D.Impulse);
            else if (user.FacingDirection == Utilities.direction.DownRight) rbd.AddForce(new Vector2(knockBack, -knockBack), ForceMode2D.Impulse);
        }//si esta en el aire right i left no añaden fuerza positiva en y
    }

    public override void itemAction(Entity target)
    {
        if ((rbd.velocity.x < -25.0f || rbd.velocity.x > 25.0f) || (rbd.velocity.y < -25.0f || rbd.velocity.y > 25.0f))
        {
            target.State = Utilities.state.Hit;
            StartCoroutine(target.receiveHit(damage, knockBack, this));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity target = other.GetComponentInParent<Entity>();
        if(target)
        {
            itemAction(target);
        }
    }

}
