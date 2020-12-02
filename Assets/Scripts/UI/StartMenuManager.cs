using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartMenuManager : MonoBehaviour
{

    private GameObject fade; //GO that contains de fade image.
    private GameObject mainText;
    private Sequence startSceneSequence;

    public bool isFaded = true, CanStart = false;//if the fade image alpha is 1, isFaded, else is false.

    private Image fade_img;
    private Text mainText_text;
    public Image logo;

    void Awake()
    {
        fade = GameObject.Find("fade");
        mainText = GameObject.Find("maintext");

        fade_img = fade.GetComponent<Image>();
        mainText_text = mainText.GetComponent<Text>();
        Invoke("playScheduledMusic", 2f);
        Invoke("textappear", 2.5f);
        initStartSequence();
        
    }

    void Start()
    {
        mainText_text.text = "Press any button to start";//set the maintext value.
        Utilities.fadeIn(fade_img, 0f, 1f);
        isFaded = false;

    }
    void Update()
    {
        textScale(mainText_text);

        if (Input.anyKeyDown && !isFaded && CanStart)
        {
            Utilities.fadeOut(fade_img, 1f, 1f);
            isFaded = true;
            SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["clickSelectButtonSFX"];
            SoundManager.instance.SFXSource.Play();
        }
        if ((fade_img.color.a == 1f) && (isFaded)) SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Gets the rectTransform of the main text, while the localscale is 1f, scale it to 0.9f, else if the localscale is 0.9f scale it to 1f.
    /// </summary>
    /// <param Text="text"></param>
    void textScale(Text text)
    {
        RectTransform text_rect = text.GetComponent<RectTransform>();
        if (text_rect.localScale == new Vector3(1f, 1f, 1f)) text_rect.DOScale(0.9f, 0.5f);
        else if (text_rect.localScale == new Vector3(0.9f, 0.9f, 0.9f)) text_rect.DOScale(1f, 0.5f);
    }

    void initStartSequence()
    {
        startSceneSequence = DOTween.Sequence();
        startSceneSequence.Append(Camera.main.transform.DOMoveY(0f, 1f, false));
        startSceneSequence.Append(logo.gameObject.GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f));
        startSceneSequence.Append(logo.GetComponent<RectTransform>().DOScale(1f, 0f));
    }

    void playScheduledMusic()
    {
        SoundManager.instance.musicSource.clip = SoundManager.instance.audioclips["mainMenuMusic"];
        SoundManager.instance.musicSource.Play();

    }

    void textappear()
    {
        mainText.GetComponent<Text>().DOFade(1, 1f);
        CanStart = true;
    }

}
