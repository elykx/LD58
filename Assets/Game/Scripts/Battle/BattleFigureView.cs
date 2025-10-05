using System.Collections;
using UnityEngine;

public class BattleFigureView : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public string FigureId;

    [Header("UI элементы")]
    public SpriteRenderer selectionHighlight; // Подсветка выбора
    public SpriteRenderer activeHighlight; // Подсветка активного бойца
    public SpriteRenderer targetHighlight; // Подсветка возможной цели

    private Vector3 originalPosition;
    private Vector3 originalScale;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
    }

    public void Initialize(string figureId)
    {
        FigureId = figureId;
        Figure figure = G.figureManager.GetFigure(figureId);


        originalPosition = transform.localPosition;

        if (spriteRenderer != null && figure.sprite != null)
        {
            spriteRenderer.sprite = figure.sprite;
        }

        // Отзеркаливаем врагов
        if (figure.isEnemy)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }

        // UpdateHealthBar();
        SetSelectionHighlight(false);
        SetActiveHighlight(false);
        SetTargetHighlight(false);
    }

    void OnMouseEnter()
    {
        if (G.battleSystem != null)
        {
            SetSelectionHighlight(true);
        }
    }

    void OnMouseExit()
    {
        if (G.battleSystem != null)
        {
            SetSelectionHighlight(false);
        }
    }

    void OnMouseDown()
    {
        if (G.battleSystem != null)
        {
            // G.battleSystem.OnTargetSelected(this);
        }
    }

    public void SetSelectionHighlight(bool active)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.enabled = active;
        }
    }

    public void SetActiveHighlight(bool active)
    {
        if (activeHighlight != null)
        {
            activeHighlight.enabled = active;
        }
    }

    public void SetTargetHighlight(bool active)
    {
        if (targetHighlight != null)
        {
            targetHighlight.enabled = active;
        }
    }

    // Анимация атаки
    public IEnumerator AttackAnimation(Vector3 targetPosition)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, targetPosition, 0.5f);

        // Движение к цели
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, midPos, t);
            yield return null;
        }

        // Возврат
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(midPos, startPos, t);
            yield return null;
        }

        transform.position = startPos;
    }

    public void PlayHitAnimation()
    {
        StartCoroutine(HitFlash());
        StartCoroutine(Shake());
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer != null)
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = original;
        }
    }

    IEnumerator Shake()
    {
        float duration = 0.2f;
        float magnitude = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void PlayDeathAnimation()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color original = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(original.r, original.g, original.b, alpha);

            // Уменьшаем размер
            float scale = Mathf.Lerp(1f, 0.5f, elapsed / duration);
            transform.localScale = originalScale * scale;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    // Показываем летящий текст урона/хила
    public void ShowDamageText(int amount, bool isHeal = false)
    {
        StartCoroutine(FloatingText(amount.ToString(), isHeal ? Color.green : Color.red));
    }

    IEnumerator FloatingText(string text, Color color)
    {
        // Создаем временный текстовый объект
        GameObject textObj = new GameObject("DamageText");
        textObj.transform.position = transform.position + Vector3.up * 0.5f;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 20;
        textMesh.color = color;
        textMesh.anchor = TextAnchor.MiddleCenter;

        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = textObj.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            textObj.transform.position = startPos + Vector3.up * t;
            textMesh.color = new Color(color.r, color.g, color.b, 1f - t);

            yield return null;
        }

        Destroy(textObj);
    }
}