using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SwordMovement : MonoBehaviour {

    public float timeTogo;
    public Vector2 startPosition, mainPosition;

	// Use this for initialization
	void Start () {
        Invoke("doMovement", timeTogo);
	}
	
    void doMovement()
    {
        gameObject.GetComponent<RectTransform>().DOLocalMove(mainPosition, 0.1f);
    }
}
