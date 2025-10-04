using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipTitle;
    public string tooltipContent;

    [Header("Style")]
    [SerializeField] private TooltipStyle customStyle;
    [SerializeField] private bool useCustomStyle = false;

    [Header("Raycast Settings")]
    [SerializeField] private bool useManualRaycast = true; // Включить ручной raycast
    [SerializeField] private LayerMask tooltipLayerMask = -1; // Все слои по умолчанию

    private bool isMouseOver = false;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Ручная проверка raycast для объектов с коллайдерами
        if (useManualRaycast)
        {
            bool wasOver = isMouseOver;
            isMouseOver = IsMouseOverThis();

            // Вошли в зону
            if (isMouseOver && !wasOver)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    ShowTooltip();
                }
            }
            // Вышли из зоны
            else if (!isMouseOver && wasOver)
            {
                HideTooltip();
            }
        }
    }

    private bool IsMouseOverThis()
    {
        if (cam == null) return false;

        Vector2 mousePos = GetMouseWorldPos();

        // Raycast по указанным слоям
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, 0f, tooltipLayerMask);

        // Проверяем все попадания
        foreach (var hit in hits)
        {
            // Ищем TooltipTrigger в попаданиях
            TooltipTrigger trigger = hit.collider.GetComponent<TooltipTrigger>();
            if (trigger != null && trigger == this)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);
    }

    // Для объектов с коллайдерами (старая система, если useManualRaycast = false)
    private void OnMouseEnter()
    {
        if (useManualRaycast) return; // Игнорируем, если используем ручной raycast

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            isMouseOver = true;
            ShowTooltip();
        }
    }

    private void OnMouseExit()
    {
        if (useManualRaycast) return;

        isMouseOver = false;
        HideTooltip();
    }

    // Для UI элементов
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useManualRaycast) return; // UI не нуждается в ручном raycast

        isMouseOver = true;
        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useManualRaycast) return;

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