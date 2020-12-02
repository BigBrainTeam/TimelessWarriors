using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : PickableItem {

    public GameObject rope;

    protected override void initializeComponents()
    {
        base.initializeComponents();
        damage = 5;
        velocity = 30;
        knockBack = 5;
        itemType = type.hook;
    }
}
