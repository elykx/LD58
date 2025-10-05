using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipShower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 5)]
    public string tooltipText;
    
    // Для UI элементов
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UITooltip.Instance != null)
        {
            UITooltip.Instance.Show(tooltipText, transform.position, isUIElement: true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (UITooltip.Instance != null)
        {
            UITooltip.Instance.Hide();
        }
    }
    
    // Для 3D/2D объектов с коллайдерами
    private void OnMouseEnter()
    {
        if (UITooltip.Instance != null)
        {
            UITooltip.Instance.Show(tooltipText, transform.position, isUIElement: false);
        }
    }
    
    private void OnMouseExit()
    {
        if (UITooltip.Instance != null)
        {
            UITooltip.Instance.Hide();
        }
    }
}