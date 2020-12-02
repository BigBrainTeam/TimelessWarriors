using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateValues : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        switch(transform.parent.name)
        {
            case "masterSlider": SoundManager.instance.setMasterVolume(gameObject.GetComponentInParent<Slider>().value); break;
            case "musicSlider": SoundManager.instance.setMusicVolume(gameObject.GetComponentInParent<Slider>().value); break;
            case "sfxSlider": SoundManager.instance.setSFXVolume(gameObject.GetComponentInParent<Slider>().value); break;
            case "voicesSlider": SoundManager.instance.setVoicesVolume(gameObject.GetComponentInParent<Slider>().value); break;
        }

        gameObject.GetComponent<Text>().text = ((int)((gameObject.GetComponentInParent<Slider>().value) * 10 / 4) + 125.0f).ToString();
    }
}


