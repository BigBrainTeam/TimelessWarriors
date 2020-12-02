using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pirate : Speciality {

    float chargeAmount;
    [SerializeField]
    int timeToDecrease;
    Image specialityImage;

    protected override void initComponents()
    {
        base.initComponents();
        chargeAmount = 0;
        specialityImage = instanceUI.transform.GetChild(1).GetComponent<Image>();
    }

    void updateChargeUI()
    {
        specialityImage.fillAmount = chargeAmount;
    }

    protected void increaseCharge()
    {
        chargeAmount += 0.34f;
        updateChargeUI();
    }

    IEnumerator startToDecrease()
    {
        int counter = 0;
        while(counter < timeToDecrease)
        {
            yield return new WaitForSeconds(1);
            counter++;
        }
        while (chargeAmount > 0)
        {
            chargeAmount -= 0.34f;
            updateChargeUI();
            yield return new WaitForSeconds(1);
        }
    }

    public void pistolHit(Entity target)
    {
        StopAllCoroutines();
        StartCoroutine(startToDecrease());

        if (chargeAmount < 1) increaseCharge();
        else burnEntity(target);
    }

    private void burnEntity(Entity target)
    {
        chargeAmount = 0;
        updateChargeUI();
        target.setStatus(Utilities.statusCondition.BURN);
    }

    public void waterHit(Entity target)
    {
        if (target.status == Utilities.statusCondition.BURN) target.setStatus(Utilities.statusCondition.SLOW);
    }

    
}
