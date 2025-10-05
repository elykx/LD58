using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI money;

    private void Awake()
    {
        G.ui = this;
    }


    public static Vector2 MousePositionToCanvasPosition(Canvas canvas, RectTransform rectTransform)
    {
        Vector2 localPoint;
        Vector2 screenPosition = Input.mousePosition;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            uiCamera,
            out localPoint
        );

        return localPoint;
    }
    public Vector2 MousePos()
    {
        return MousePositionToCanvasPosition(gameObject.GetComponent<Canvas>(), gameObject.GetComponent<RectTransform>());
    }

}

