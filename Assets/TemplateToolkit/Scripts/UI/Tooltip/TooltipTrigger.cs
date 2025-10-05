using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Content")]
    [TextArea(3, 5)]
    [SerializeField] private string tooltipContent = "Tooltip text";

    [Header("Settings")]
    [SerializeField] private bool useRichText = true; // Поддержка Rich Text для форматирования
    [SerializeField] private bool isWorldObject = false; // true для 3D/2D объектов, false для UI

    private bool isMouseOver = false;

    private void Start()
    {
        // Автоматическое определение типа объекта
        if (!isWorldObject)
        {
            // Проверяем, является ли объект UI элементом
            if (GetComponent<RectTransform>() == null)
            {
                isWorldObject = true;
            }
        }

        // Проверка наличия необходимых компонентов для world объектов
        if (isWorldObject)
        {
            Collider col = GetComponent<Collider>();
            Collider2D col2D = GetComponent<Collider2D>();

            if (col == null && col2D == null)
            {
                Debug.LogWarning($"TooltipTrigger на объекте '{gameObject.name}' требует Collider или Collider2D для работы с world объектами!");
            }
        }
    }

    #region UI Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isWorldObject)
        {
            ShowTooltip();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isWorldObject)
        {
            HideTooltip();
        }
    }
    #endregion

    #region World Object Events
    private void OnMouseEnter()
    {
        if (isWorldObject && !IsPointerOverUI())
        {
            ShowTooltip();
        }
    }

    private void OnMouseExit()
    {
        if (isWorldObject)
        {
            HideTooltip();
        }
    }

    private void OnMouseOver()
    {
        // Проверяем, не перешел ли курсор на UI
        if (isWorldObject && isMouseOver && IsPointerOverUI())
        {
            HideTooltip();
        }
    }
    #endregion

    #region Tooltip Control
    private void ShowTooltip()
    {
        if (TooltipManager.Instance == null) return;
        if (string.IsNullOrEmpty(tooltipContent)) return;
        if (isMouseOver) return; // Уже показан

        string content = ProcessContent(tooltipContent);
        TooltipManager.Instance.ShowTooltip(content);
        isMouseOver = true;
    }

    private void HideTooltip()
    {
        if (TooltipManager.Instance == null) return;
        if (!isMouseOver) return; // Уже скрыт

        TooltipManager.Instance.HideTooltip();
        isMouseOver = false;
    }

    private string ProcessContent(string content)
    {
        // Обработка контента перед показом
        if (!useRichText)
        {
            // Удаляем Rich Text теги если они отключены
            content = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);
        }

        // Можно добавить дополнительную обработку
        // Например, замену переменных: {playerName} -> ActualPlayerName

        return content;
    }
    #endregion

    #region Public Methods
    public void SetTooltipContent(string content)
    {
        tooltipContent = content;

        // Обновляем тултип если он уже показан
        if (isMouseOver && TooltipManager.Instance != null)
        {
            string processedContent = ProcessContent(content);
            TooltipManager.Instance.ShowTooltip(processedContent);
        }
    }

    public void AppendTooltipContent(string additionalContent)
    {
        tooltipContent += "\n" + additionalContent;

        if (isMouseOver && TooltipManager.Instance != null)
        {
            string processedContent = ProcessContent(tooltipContent);
            TooltipManager.Instance.ShowTooltip(processedContent);
        }
    }

    public void ClearTooltipContent()
    {
        tooltipContent = string.Empty;
        if (isMouseOver)
        {
            HideTooltip();
        }
    }

    public string GetTooltipContent()
    {
        return tooltipContent;
    }

    public void SetRichTextEnabled(bool enabled)
    {
        useRichText = enabled;

        if (isMouseOver && TooltipManager.Instance != null)
        {
            string processedContent = ProcessContent(tooltipContent);
            TooltipManager.Instance.ShowTooltip(processedContent);
        }
    }
    #endregion

    #region Lifecycle
    private void OnDisable()
    {
        if (isMouseOver)
        {
            HideTooltip();
        }
    }

    private void OnDestroy()
    {
        if (isMouseOver)
        {
            HideTooltip();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Скрываем тултип при потере фокуса приложения
        if (!hasFocus && isMouseOver)
        {
            HideTooltip();
        }
    }
    #endregion

    #region Utilities
    private bool IsPointerOverUI()
    {
        // Проверка для разных систем ввода
        if (EventSystem.current == null) return false;

        // Для мыши
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        // Для тача (мобильные устройства)
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return true;

        return false;
    }
    #endregion
}