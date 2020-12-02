using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Special controller for BlackTemplar. Controls the Shadow.
/// </summary>
public class BlackTemplar : Speciality
{
    public bool hasShadow;
    public TemplarShadow shadow;
    Image[] shadowImages;
    public Vector3 shadowPos;
    protected override void initComponents()
    {
        base.initComponents();
        hasShadow = true;
        shadowPos = shadow.transform.localPosition;
        shadowImages = instanceUI.GetComponentsInChildren<Image>();
    }

    void updateChargesUI()
    {
        if (hasShadow) shadowImages[1].enabled = true;
        else shadowImages[1].enabled = false;
    }

    public void shadowAttack(Attack atk)
    {
        shadow.doAttack(atk.animationName);
        hasShadow = true;
        updateChargesUI();
    }

    public void specialDown()
    {
        if (!hasShadow) teleport();
        else drop();
    }
    public void specialUp(Attack atk)
    {
        shadow.doAttack(atk.animationName);
    }
    public void teleport()
    {
        shadow.teleport();
        hasShadow = true;
        user.transform.position = shadow.transform.position;
        shadow.transform.SetParent(user.transform, true);
        shadow.transform.localPosition = shadowPos;
        updateChargesUI();
    }

    public void specialUpEnd()
    {
        hasShadow = true;
        user.state = Utilities.state.Air;
        shadow.teleport();
        user.transform.position = shadow.transform.position;
        user.RigidBody.velocity = new Vector2(0, 0);
        shadow.transform.SetParent(user.transform, true);
        shadow.transform.localPosition = shadowPos;
        updateChargesUI();
    }

    public void resetShadow()
    {
        shadow.resetComps();
        hasShadow = true;
        updateChargesUI();
    }

    internal void drop()
    {
        hasShadow = false;
        shadow.drop();
        updateChargesUI();
    }
    public void specialSide()
    {
        shadow.specialSide();
        hasShadow = true;
        updateChargesUI();
    }

}
