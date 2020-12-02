using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffCoin : PickableItem {

    enum coinType {SPEED, ATTACKDMG, ATTACKSPEED, JUMP};
    coinType Type;
    float buffdamage, buffSpeed, buffAttackSpeed, buffJump;
    SpriteRenderer sprR;
    protected override void initializeComponents()
    {
        base.initializeComponents();
        damage = 0;
        knockBack = 0;
        velocity = 0;
        sprR = gameObject.GetComponent<SpriteRenderer>();
        Type = (coinType)Random.Range(0, 4);
        Debug.Log(Type);
        chooseCoinType();
    }
    new void Update()
    {
        base.Update();
    }
    void chooseCoinType()
    {
        switch (Type)
        {
            case coinType.SPEED: sprR.color = new Color32(255, 255, 255, 255); break;
            case coinType.ATTACKDMG: sprR.color = new Color32(255, 0, 70, 255); break;
            case coinType.ATTACKSPEED: sprR.color = new Color32(255, 150, 70, 255); break;
            case coinType.JUMP: sprR.color = new Color32(0, 255, 235,255); break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponentInParent<Entity>())
        {
            //Entity target = other.GetComponentInParent<Entity>();
            switch(Type)
            {
                case coinType.SPEED: /*buffspeed*/ break;
                case coinType.ATTACKDMG:/*buffdmg*/  break;
                case coinType.ATTACKSPEED: /*buffatspeed*/  break;
                case coinType.JUMP: /*buffextrajump*/  break;
            }
        }
    }
}
