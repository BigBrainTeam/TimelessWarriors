using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
public class Collider2DRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
    Collider2D myCollider;
    RectTransform rectTransform;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    #region ICanvasRaycastFilter implementation
    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        var worldPoint = Vector3.zero;
        var isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            screenPos,
            eventCamera,
            out worldPoint
        );
        if (isInside)
        {
            isInside = myCollider.OverlapPoint(worldPoint);
        }  
        return isInside;
    }
    #endregion

    private void Update()
    {
        checkHighlighting();
        checkButtonPressed();
    }

    void checkHighlighting()
    {
        if (InputControl.m_State == InputControl.eInputState.MouseKeyboard)
        {
            if (IsRaycastLocationValid(Input.mousePosition, Camera.main))
            {
                if (!gameObject.GetComponent<Buttons>().highlighted)
                {                   
                    MenuManager.instance.players[0].selectedButton = gameObject.GetComponent<Buttons>();
                    if (MenuManager.instance.players[0].selectedButton.GetComponent<WarriorSelectionButtons>() || MenuManager.instance.players[0].selectedButton.GetComponent<MapSelectionButton>())
                    {
                        MenuManager.instance.players[0].selectedButton.buttonHighlight(0);
                    }else
                    {
                        gameObject.GetComponent<Buttons>().buttonHighlight();
                    }
                }
            }
            else
            {
                if (gameObject.GetComponent<Buttons>().highlighted)
                {
                    gameObject.GetComponent<Buttons>().buttonHighlightOff();
                }

            }
        }
    }
    void checkButtonPressed()
    {
        if (Input.GetMouseButtonDown(0) && IsRaycastLocationValid(Input.mousePosition, Camera.main))
        {
            if (MenuManager.instance.players[0].selectedButton.GetComponent<WarriorSelectionButtons>() || MenuManager.instance.players[0].selectedButton.GetComponent<MapSelectionButton>())
            {
                MenuManager.instance.players[0].selectedButton.onClickButton(0);
            } else gameObject.GetComponent<Buttons>().onClickButton();
        }
    }
}
