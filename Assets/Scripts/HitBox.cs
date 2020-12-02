using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    AttacksController attacks;

    public void Start()
    {
        attacks = transform.parent.GetComponent<AttacksController>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        attacks.hit(collision);
    }

    public void deleteEffect()
    {
        Destroy(this.gameObject);
    }
}
