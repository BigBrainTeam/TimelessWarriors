using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : ThrowableItem {
    public GameObject bombExplosion;
    public GameObject mineBombExplosion;
    public LayerMask hurtBoxLayer;
    [SerializeField] BoxCollider2D[] mycols;
    bool sticked = false;

    protected override void initializeComponents()
    {
        base.initializeComponents();
        mycols = GetComponents<BoxCollider2D>();
        rbd.constraints = RigidbodyConstraints2D.FreezeRotation;
        damage = 15;
        velocity = 30;
        knockBack = 5;
        itemType = type.explosive;
    }
    public override void itemAction()
    {
        Collider2D[] targetsColliders = Physics2D.OverlapCircleAll(transform.position, 2, hurtBoxLayer);
        isUsed = false;
        if (sticked)
        {
            GameObject mineExplosionID = Instantiate(mineBombExplosion, transform.position, transform.rotation);
            Destroy(mineExplosionID, 25f * Time.deltaTime);
        }
        else
        {
           GameObject bombExplosionID =  Instantiate(bombExplosion, transform.position, transform.rotation);
            Destroy(bombExplosionID, 50f * Time.deltaTime);
        }
        foreach (Collider2D c in targetsColliders)
        {
            if (c.gameObject.name == "HurtBox")
            {
                Entity currentEntity = c.gameObject.GetComponentInParent<Entity>();
                currentEntity.State = Utilities.state.Hit;
                StartCoroutine(currentEntity.receiveHit(damage, knockBack, this));
            }
        }
        GameManager.Instance.currentFight.onObjectDestroyed(this.gameObject);
        Destroy(this.gameObject);

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed || sticked) itemAction();
    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if(isUsed)stickOnWall(other);
    }

    public void stickOnWall(Collision2D wall)
    {
        rbd.velocity = Vector3.zero;
        rbd.constraints = RigidbodyConstraints2D.FreezeAll;
        if (!sticked)
        {
            if (mycols[0].IsTouching(wall.collider))
            {
                transform.localEulerAngles = new Vector3(0, 0, 180f);
                sticked = true;
                return;
            }else if(mycols[1].IsTouching(wall.collider))
            {
                transform.localEulerAngles = new Vector3(0, 0, 0f);
                sticked = true;
                return;
            }

            if (mycols[2].IsTouching(wall.collider) || mycols[3].IsTouching(wall.collider))
            {
                if (wall.transform.position.x < transform.position.x)
                {
                    transform.localEulerAngles = new Vector3(0, 0, -90f);
                    transform.position -= new Vector3(0.5f,0f,0f);
                    sticked = true;
                    Debug.Log("Left");
                    return;
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 90f);
                    sticked = true;
                    Debug.Log("Right");
                    return;
                }
            }
        }
       
    }

    public void checkCollisionSide( Collision2D wall)
    {
      
    }
}
