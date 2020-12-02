
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : PickableItem {

    protected Utilities.direction thrownDirection;
    public virtual void onThrow()
    {

    }

    public void setDirection(Utilities.direction dir)
    {
        thrownDirection = dir;
    }

    public Utilities.direction getDirection()
    {
        return thrownDirection;
    }

    new void Update()
    {
        base.Update();
    }
}
