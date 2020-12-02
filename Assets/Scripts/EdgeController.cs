using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeController: MonoBehaviour {
    int flip;
    Collider2D user;
    void Start()
    {
        if (name == "LeftEdge") flip = 1;
        else flip = -1;
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Character")
        {
            if (!user)
            {
                user = col;
                col.gameObject.GetComponent<Character>().setEdgeGrabbing(transform.position, flip, this);
            }         
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other == user) user = null;
    }
}
