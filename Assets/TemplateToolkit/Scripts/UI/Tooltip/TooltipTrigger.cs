using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipTitle;
    public string tooltipContent;

    [Header("Style")]
    [SerializeField] private TooltipStyle customStyle;
    [SerializeField] private bool useCustomStyle = false;

    private bool isMouseOver = false;

    // Для объектов с коллайдерами
    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())  // Игнорировать, если курсор над UI
        {
            isMouseOver = true;
            ShowTooltip();
        }
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (TooltipManager.Instance != null)
        {
            if (useCustomStyle && customStyle != null)
            {
                TooltipManager.Instance.ShowTooltip(tooltipTitle, tooltipContent, customStyle);
            }
            else
            {
                TooltipManager.Instance.ShowTooltip(tooltipTitle, tooltipContent);
            }
        }
    }

    private void HideTooltip()
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    private void OnDisable()
    {
        if (isMouseOver && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
            isMouseOver = false;
        }
    }
}