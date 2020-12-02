using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class rotationTime : MonoBehaviour {

    public float speed;
    bool stopMove = false;
    // Use this for initialization
    void Start()
    {
        Invoke("stopMovelol", 2f);
        StartCoroutine("rotatelikegod");
    }

    IEnumerator rotatelikegod()
    {
        while(!stopMove)
        {
            gameObject.transform.Rotate(0f, 0f, speed);
            yield return new WaitForEndOfFrame();
        }
    }

    void stopMovelol()
    {
        stopMove = true;
    }
}
