using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PickableItem : Item {

    protected bool isPicked, isUsed;
    protected Canvas mycanvas;
    protected Image buttonImage;
    protected RectTransform buttonRect;
    public Collider2D[] entitiesInRange;
    public LayerMask entity;


    protected override void initializeComponents()
    {
        base.initializeComponents();
        isPicked = false;
        isUsed = false;
        mycanvas = GetComponentInChildren<Canvas>();
        mycanvas.enabled = true;
        buttonImage = GetComponentInChildren<Image>();
        buttonRect = buttonImage.rectTransform;
    }

    protected override void Update()
    {
        checkEntitiesInRange();
    }

    public virtual void pickUp(Transform p)
    {
        rbd.isKinematic = true;
        rbd.velocity = new Vector2(0, 0);
        transform.parent = p;
        transform.localPosition = new Vector2(0, 0);
    }

    public Canvas getPickUpCanvas()
    {
        return mycanvas;
    }

    public void setIsUsed(bool _isUsed)
    {
        isUsed = _isUsed;
    }

    public bool getIsUsed()
    {
        return isUsed;
    }
    public bool getIsPicked()
    {
        return isPicked;
    }

    public void setIsPicked(bool _isPicked)
    {
        isPicked = _isPicked;
    }

    public virtual void checkEntitiesInRange()
    {
        entitiesInRange = Physics2D.OverlapCircleAll(transform.position, 1f, entity);
        if (entitiesInRange.Length != 0 && !isPicked && !isUsed)
        {
            generatePickUpCanvas();
        }
        else cleanPickUpCanvas();
    }


    public void generatePickUpCanvas()
    {
        buttonImage.DOFade(1, 0.2f);
        buttonRect.DOScale(1.5f, 0.3f);
    }

    public void cleanPickUpCanvas()
    {
        buttonImage.DOFade(0, 0.2f);
        buttonRect.DOScale(1, 0.3f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
