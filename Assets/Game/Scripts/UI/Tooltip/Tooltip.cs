using UnityEngine;
using TMPro;
using LitMotion;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    public static UITooltip Instance { get; private set; }
    
    [Header("References")]
    public TMP_Text tooltipText;
    public CanvasGroup canvasGroup;
    public RectTransform tooltipRect;
    public RectTransform backgroundRect; // RectTransform картинки
    
    [Header("Settings")]
    public Vector2 offset = new Vector2(10, 10);
    public float maxWidth = 300f;
    public float padding = 15f;
    
    [Header("Animation")]
    public float fadeDuration = 0.25f;
    public Vector3 showScale = Vector3.one;
    public Vector3 hideScale = new Vector3(0.8f, 0.8f, 1f);
    
    private MotionHandle alphaMotion;
    private MotionHandle scaleMotion;
    private Canvas canvas;
    private RectTransform canvasRect;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Найдено несколько UITooltip в сцене! Удаляем дубликат.");
            Destroy(gameObject);
            return;
        }
        
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        
        if (tooltipRect == null)
            tooltipRect = GetComponent<RectTransform>();
        
        tooltipRect.pivot = new Vector2(0, 0);
        
        canvasGroup.alpha = 0;
        tooltipRect.localScale = hideScale;
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
        if (alphaMotion.IsActive()) alphaMotion.Cancel();
        if (scaleMotion.IsActive()) scaleMotion.Cancel();
    }
    
    public void Show(string text, Vector3 worldPosition, bool isUIElement = false)
    {
        if (alphaMotion.IsActive()) alphaMotion.Cancel();
        if (scaleMotion.IsActive()) scaleMotion.Cancel();
        
        tooltipText.text = text;
        
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        
        UpdateSize();
        PositionTooltip(worldPosition, isUIElement);
        
        alphaMotion = LMotion.Create(canvasGroup.alpha, 1f, fadeDuration)
            .Bind(x => canvasGroup.alpha = x);
        
        scaleMotion = LMotion.Create(tooltipRect.localScale, showScale, fadeDuration)
            .WithEase(Ease.OutBack)
            .Bind(x => tooltipRect.localScale = x);
    }
    
    public void Hide()
    {
        if (alphaMotion.IsActive()) alphaMotion.Cancel();
        if (scaleMotion.IsActive()) scaleMotion.Cancel();
        
        alphaMotion = LMotion.Create(canvasGroup.alpha, 0f, fadeDuration)
            .WithOnComplete(() => 
            {
                if (gameObject != null)
                    gameObject.SetActive(false);
            })
            .Bind(x => canvasGroup.alpha = x);
        
        scaleMotion = LMotion.Create(tooltipRect.localScale, hideScale, fadeDuration)
            .WithEase(Ease.InBack)
            .Bind(x => tooltipRect.localScale = x);
    }
    
    private void UpdateSize()
    {
        // Временно устанавливаем большую ширину для расчёта
        tooltipText.rectTransform.sizeDelta = new Vector2(maxWidth - padding * 2, 10000);
        
        // Принудительно обновляем
        Canvas.ForceUpdateCanvases();
        tooltipText.ForceMeshUpdate();
        
        // Получаем реальный размер текста
        float textWidth = Mathf.Min(tooltipText.preferredWidth, maxWidth - padding * 2);
        float textHeight = tooltipText.preferredHeight;
        
        // Размер текста
        tooltipText.rectTransform.sizeDelta = new Vector2(textWidth, textHeight);
        tooltipText.rectTransform.anchoredPosition = new Vector2(padding, padding);
        
        // Общий размер тултипа
        Vector2 tooltipSize = new Vector2(textWidth + padding * 2, textHeight + padding * 2);
        tooltipRect.sizeDelta = tooltipSize;
        
        // Растягиваем картинку на весь размер
        if (backgroundRect != null)
        {
            backgroundRect.sizeDelta = tooltipSize;
            backgroundRect.anchoredPosition = Vector2.zero;
        }
    }
    
    private void PositionTooltip(Vector3 worldPosition, bool isUIElement)
    {
        Vector2 screenPos;
        
        if (isUIElement)
        {
            screenPos = Input.mousePosition;
        }
        else
        {
            screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        }
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );
        
        tooltipRect.anchoredPosition = localPoint + offset;
        ClampToScreen();
    }
    
    private void ClampToScreen()
    {
        Vector2 pos = tooltipRect.anchoredPosition;
        Vector2 size = tooltipRect.sizeDelta;
        Vector2 canvasSize = canvasRect.sizeDelta;
        
        float halfCanvasWidth = canvasSize.x / 2;
        float halfCanvasHeight = canvasSize.y / 2;
        
        if (pos.x + size.x > halfCanvasWidth)
            pos.x = halfCanvasWidth - size.x - 5;
        
        if (pos.x < -halfCanvasWidth)
            pos.x = -halfCanvasWidth + 5;
        
        if (pos.y + size.y > halfCanvasHeight)
            pos.y = halfCanvasHeight - size.y - 5;
        
        if (pos.y < -halfCanvasHeight)
            pos.y = -halfCanvasHeight + 5;
        
        tooltipRect.anchoredPosition = pos;
    }
}