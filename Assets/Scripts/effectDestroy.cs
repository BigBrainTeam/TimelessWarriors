using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class effectDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(delay());
    }
	
	// Update is called once per frame
    IEnumerator delay()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
