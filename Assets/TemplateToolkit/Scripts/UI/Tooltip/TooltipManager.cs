using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private LayoutElement layoutElement;

    [Header("Settings")]
    [SerializeField] private float showDelay = 0.5f;
    [SerializeField] private float maxWidth = 300f;
    [SerializeField] private float padding = 10f;
    [SerializeField] private Vector2 offset = new Vector2(10, -10); // Смещение от курсора

    private RectTransform tooltipRect;
    private RectTransform canvasRect;
    private Canvas canvas;
    private float delayTimer;
    private bool isWaitingToShow;
    private string pendingContent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeComponents();
        HideTooltip();
    }

    private void InitializeComponents()
    {
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        // Настройка ContentSizeFitter
        if (contentSizeFitter == null)
        {
            contentSizeFitter = tooltipPanel.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = tooltipPanel.AddComponent<ContentSizeFitter>();
            }
        }
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Настройка LayoutElement для ограничения ширины
        if (layoutElement == null)
        {
            layoutElement = tooltipText.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = tooltipText.gameObject.AddComponent<LayoutElement>();
            }
        }
        layoutElement.preferredWidth = maxWidth;

        // Настройка текста
        tooltipText.textWrappingMode = TextWrappingModes.Normal;
        tooltipText.overflowMode = TextOverflowModes.Overflow;

        // Настройка якорей для правильного позиционирования
        tooltipRect.pivot = new Vector2(0, 1); // Левый верхний угол
    }

    private void Update()
    {
        if (isWaitingToShow)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                isWaitingToShow = false;
                ActuallyShowTooltip();
            }
        }

        if (tooltipPanel.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        // Конвертируем позицию мыши в локальные координаты Canvas
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchoredPosition
        );

        // Применяем смещение
        anchoredPosition += offset;

        // Форсируем обновление размера тултипа
        Canvas.ForceUpdateCanvases();

        // Получаем актуальный размер тултипа
        float tooltipWidth = layoutElement.preferredWidth + padding * 2;
        float tooltipHeight = tooltipText.preferredHeight + padding * 2;

        // Проверяем и корректируем позицию, чтобы тултип не выходил за границы экрана
        float halfCanvasWidth = canvasRect.rect.width / 2;
        float halfCanvasHeight = canvasRect.rect.height / 2;

        // Проверка правой границы
        if (anchoredPosition.x + tooltipWidth > halfCanvasWidth)
        {
            // Показываем слева от курсора
            anchoredPosition.x = anchoredPosition.x - offset.x * 2 - tooltipWidth;
        }

        // Проверка левой границы
        if (anchoredPosition.x < -halfCanvasWidth)
        {
            anchoredPosition.x = -halfCanvasWidth + padding;
        }

        // Проверка верхней границы
        if (anchoredPosition.y > halfCanvasHeight)
        {
            anchoredPosition.y = halfCanvasHeight - padding;
        }

        // Проверка нижней границы
        if (anchoredPosition.y - tooltipHeight < -halfCanvasHeight)
        {
            // Показываем над курсором
            anchoredPosition.y = anchoredPosition.y - offset.y * 2 + tooltipHeight;
        }

        tooltipRect.anchoredPosition = anchoredPosition;
    }

    public void ShowTooltip(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            HideTooltip();
            return;
        }

        pendingContent = content;
        delayTimer = showDelay;
        isWaitingToShow = true;
    }

    private void ActuallyShowTooltip()
    {
        tooltipText.text = pendingContent;

        // Форсируем обновление layout
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);

        tooltipPanel.SetActive(true);
        UpdatePosition();
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        isWaitingToShow = false;
        pendingContent = string.Empty;
    }

    public void SetMaxWidth(float width)
    {
        maxWidth = width;
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = maxWidth;
        }
    }

    public void SetShowDelay(float delay)
    {
        showDelay = delay;
    }
}