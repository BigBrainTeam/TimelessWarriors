using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class VideoSettingsButton : Buttons {

    public Vector3 highlightedScale;
    public bool isOnDropDown;

    public override void buttonHighlight()
    {
        highlightedScale = new Vector3(1.08f, 1.08f, 1.08f);
        gameObject.GetComponent<RectTransform>().DOScale(highlightedScale, 0.2f).SetUpdate(true);
        base.buttonHighlight();
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["selectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
    }

    public override void buttonHighlightOff()
    {
        gameObject.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetUpdate(true);
        base.buttonHighlightOff();
    }

    public override void onClickButton()
    {
        SoundManager.instance.SFXSource.clip = SoundManager.instance.audioclips["clickSelectButtonSFX"];
        SoundManager.instance.SFXSource.Play();
        buttonHighlight();
        isOnDropDown = true;
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
