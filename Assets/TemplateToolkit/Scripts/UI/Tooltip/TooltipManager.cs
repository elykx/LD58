using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipTitle;
    [SerializeField] private TextMeshProUGUI tooltipContent;
    [SerializeField] private Image backgroundImage;

    [Header("Default Style")]
    [SerializeField] private TooltipStyle defaultStyle;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(15, 0);
    [SerializeField] private float showDelay = 0.3f;
    [SerializeField] private TooltipPosition defaultPosition = TooltipPosition.Right;
    [SerializeField] private float maxWidth = 300f;
    [SerializeField] private float closerOffset = 5f;

    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private float delayTimer;
    private bool isTimerRunning;
    private LayoutElement titleLayoutElement;
    private LayoutElement contentLayoutElement;
    private VerticalLayoutGroup layoutGroup;


    public enum TooltipPosition
    {
        Right,
        Left,
        Top,
        Bottom
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        rectTransform = tooltipPanel.GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        layoutGroup = tooltipPanel.GetComponent<VerticalLayoutGroup>();

        InitializeTextComponents();
        HideTooltip();
    }

    private void InitializeTextComponents()
    {
        if (tooltipTitle != null)
        {
            tooltipTitle.textWrappingMode = TextWrappingModes.Normal;
            tooltipTitle.overflowMode = TextOverflowModes.Overflow;

            titleLayoutElement = tooltipTitle.GetComponent<LayoutElement>();
            if (titleLayoutElement == null)
            {
                titleLayoutElement = tooltipTitle.gameObject.AddComponent<LayoutElement>();
            }
            titleLayoutElement.preferredWidth = maxWidth;
        }

        if (tooltipContent != null)
        {
            tooltipTitle.textWrappingMode = TextWrappingModes.Normal;
            tooltipContent.overflowMode = TextOverflowModes.Overflow;

            contentLayoutElement = tooltipContent.GetComponent<LayoutElement>();
            if (contentLayoutElement == null)
            {
                contentLayoutElement = tooltipContent.gameObject.AddComponent<LayoutElement>();
            }
            contentLayoutElement.preferredWidth = maxWidth;
        }
    }


    private void Update()
    {
        if (isTimerRunning)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                isTimerRunning = false;
                tooltipPanel.SetActive(true);
            }
        }

        if (tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    private void UpdateTooltipPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        Vector2 tooltipSize = rectTransform.rect.size;
        TooltipPosition position = defaultPosition;

        if (position == TooltipPosition.Right && mousePosition.x + tooltipSize.x + offset.x > screenSize.x)
        {
            position = TooltipPosition.Left;
        }

        if (position == TooltipPosition.Left && mousePosition.x - tooltipSize.x - offset.x < 0)
        {
            position = TooltipPosition.Bottom;
        }

        if (position == TooltipPosition.Bottom && mousePosition.y - tooltipSize.y - offset.y < 0)
        {
            position = TooltipPosition.Top;
        }

        if (position == TooltipPosition.Top && mousePosition.y + tooltipSize.y + offset.y > screenSize.y)
        {
            position = TooltipPosition.Right;
        }

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            mousePosition,
            parentCanvas.worldCamera,
            out localPoint
        );

        switch (position)
        {
            case TooltipPosition.Right:
                localPoint.x += closerOffset;
                break;
            case TooltipPosition.Left:
                localPoint.x -= tooltipSize.x + closerOffset;
                break;
            case TooltipPosition.Top:
                localPoint.y += closerOffset;
                break;
            case TooltipPosition.Bottom:
                localPoint.y -= tooltipSize.y + closerOffset;
                break;
        }

        rectTransform.anchoredPosition = localPoint;
        ClampTooltipPositionToScreen();
    }

    private void ClampTooltipPositionToScreen()
    {
        Vector2 tooltipSize = rectTransform.rect.size;
        Vector2 canvasSize = parentCanvas.GetComponent<RectTransform>().rect.size;
        Vector2 position = rectTransform.anchoredPosition;

        if (position.x + tooltipSize.x > canvasSize.x / 2)
        {
            position.x = canvasSize.x / 2 - tooltipSize.x;
        }

        if (position.x < -canvasSize.x / 2)
        {
            position.x = -canvasSize.x / 2;
        }

        if (position.y + tooltipSize.y > canvasSize.y / 2)
        {
            position.y = canvasSize.y / 2 - tooltipSize.y;
        }

        if (position.y < -canvasSize.y / 2)
        {
            position.y = -canvasSize.y / 2;
        }

        rectTransform.anchoredPosition = position;
    }

    public void ShowTooltip(string title, string content, TooltipStyle style = null)
    {
        TooltipStyle activeStyle = style != null ? style : defaultStyle;

        if (activeStyle != null)
        {
            ApplyStyle(activeStyle);
        }

        tooltipTitle.gameObject.SetActive(!string.IsNullOrEmpty(title));
        tooltipTitle.text = title;
        tooltipContent.text = content;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        delayTimer = showDelay;
        isTimerRunning = true;
    }
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        isTimerRunning = false;
    }

    private void ApplyStyle(TooltipStyle style)
    {
        // Фон
        if (backgroundImage != null)
        {
            backgroundImage.color = style.backgroundColor;
        }

        // Стиль заголовка
        if (tooltipTitle != null)
        {
            tooltipTitle.color = style.titleColor;
            tooltipTitle.fontSize = style.titleFontSize;
            tooltipTitle.fontStyle = style.titleFontStyle;
        }

        // Стиль контента
        if (tooltipContent != null)
        {
            tooltipContent.color = style.contentColor;
            tooltipContent.fontSize = style.contentFontSize;
            tooltipContent.fontStyle = style.contentFontStyle;
        }

        // Отступы
        if (layoutGroup != null)
        {
            layoutGroup.padding = style.Padding;
            layoutGroup.spacing = style.titleBottomPadding;
        }

    }
}
