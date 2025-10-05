using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public RectTransform target;   // Кнопка/объект для подсветки
        [TextArea] public string message; // Текст подсказки
        public Vector2 offset;         // Смещение текста
    }

    public Step[] steps;
    public Image highlight;
    public RectTransform tooltip;
    public TMP_Text messageText;
    public GameObject panel;

    public float moveSpeed = 10f;  // скорость перемещения подсветки
    public float scaleSpeed = 10f; // скорость анимации масштаба тултипа

    private int currentStep = 0;

    void Start()
    {
        if (steps.Length == 0)
        {
            Debug.LogWarning("No tutorial steps!");
            return;
        }

        ShowStep(0, true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextStep();
        }

        // Плавно двигаем highlight к цели
        if (highlight.enabled && currentStep < steps.Length && steps[currentStep].target != null)
        {
            Vector3 targetPos = steps[currentStep].target.position;
            highlight.rectTransform.position = Vector3.Lerp(highlight.rectTransform.position, targetPos, Time.deltaTime * moveSpeed);

            Vector2 targetSize = steps[currentStep].target.sizeDelta + new Vector2(20, 20);
            highlight.rectTransform.sizeDelta = Vector2.Lerp(highlight.rectTransform.sizeDelta, targetSize, Time.deltaTime * moveSpeed);
        }

        // Плавно двигаем тултип
        if (tooltip != null && currentStep < steps.Length && steps[currentStep].target != null)
        {
            Vector3 tooltipTargetPos = steps[currentStep].target.position + (Vector3)steps[currentStep].offset;
            tooltip.position = Vector3.Lerp(tooltip.position, tooltipTargetPos, Time.deltaTime * moveSpeed);

            // Масштаб тултипа
            tooltip.localScale = Vector3.Lerp(tooltip.localScale, Vector3.one, Time.deltaTime * scaleSpeed);
        }
    }

    void ShowStep(int index, bool instant = false)
    {
        if (index >= steps.Length)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);
        var step = steps[index];

        if (step.target == null)
        {
            Debug.LogWarning("Step " + index + " has null target!");
            return;
        }

        messageText.text = step.message;

        // Позиция highlight
        if (instant)
        {
            highlight.rectTransform.position = step.target.position;
            highlight.rectTransform.sizeDelta = step.target.sizeDelta + new Vector2(20, 20);
            tooltip.position = step.target.position + (Vector3)step.offset;
            tooltip.localScale = Vector3.one;
        }
    }

    void NextStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }
}
