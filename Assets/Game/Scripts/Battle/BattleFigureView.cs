using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleFigureView : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public string FigureId;

    [Header("UI элементы")]
    public SpriteRenderer selectionHighlight;
    public SpriteRenderer activeHighlight;
    public SpriteRenderer targetHighlight;

    [Header("Полосы состояния")]
    public GameObject healthBarContainer;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;

    public GameObject actionBarContainer;
    public Image actionBarFill;

    [Header("Эффекты")]
    public GameObject buffEffectPrefab;
    public GameObject debuffEffectPrefab;
    public Transform effectsContainer;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Figure figure;
    private TooltipShower tooltip;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
        tooltip = GetComponent<TooltipShower>();
    }

    private void UpdateTooltip(Figure figureInDb)
    {
        Figure data = figureInDb;
        if (tooltip != null && data != null)
        {
            // Заголовок: имя + уровень
            tooltip.tooltipText =
            $"<b><color=#FFD700>{data.name}</color></b> " +
            $"<size=80%><color=#00BFFF>[Lvl {data.lvl}]</color></size>" +
            $"<i>{data.description}</i>\n\n" +
            $"<b><color=#32CD32>Здоровье:</color></b> {data.currentHealth}/{data.maxHealth}\n" +
            $"<b><color=#DC143C>Урон:</color></b> {data.damage}\n" +
            $"<b><color=#1E90FF>Скорость:</color></b> {data.speed}\n" +
            $"<b><color=#A9A9A9>Защита:</color></b> {data.defense}\n" +
            $"<b><color=#FFD700>Стоимость:</color></b> {data.cost}"
            ;
        }
    }

    public void Initialize(string figureId)
    {
        FigureId = figureId;
        figure = G.figureManager.GetFigure(figureId);

        originalPosition = transform.localPosition;

        if (spriteRenderer != null && figure.sprite != null)
        {
            spriteRenderer.sprite = figure.sprite;
        }

        if (figure.isEnemy)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

            // Отзеркаливаем Canvas'ы обратно, чтобы UI не был перевернут
            if (healthBarContainer != null)
            {
                Vector3 hpScale = healthBarContainer.transform.localScale;
                healthBarContainer.transform.localScale = new Vector3(-hpScale.x, hpScale.y, hpScale.z);
            }

            if (actionBarContainer != null)
            {
                Vector3 actionScale = actionBarContainer.transform.localScale;
                actionBarContainer.transform.localScale = new Vector3(-actionScale.x, actionScale.y, actionScale.z);
            }

            // Если есть контейнер эффектов, его тоже отзеркаливаем
            if (effectsContainer != null)
            {
                Vector3 effectScale = effectsContainer.localScale;
                effectsContainer.localScale = new Vector3(-effectScale.x, effectScale.y, effectScale.z);
            }
        }

        UpdateHealthBar();
        UpdateActionBar(0f);
        SetSelectionHighlight(false);
        SetActiveHighlight(false);
        SetTargetHighlight(false);
        UpdateTooltip(figure);
    }

    void Update()
    {
        // Обновляем HP если фигура жива
        if (figure != null && figure.IsAlive())
        {
            UpdateTooltip(figure);
            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        if (figure == null || healthBarFill == null) return;

        float healthPercent = (float)figure.currentHealth / figure.maxHealth;
        healthBarFill.fillAmount = healthPercent;

        // Меняем цвет в зависимости от здоровья
        if (healthPercent > 0.6f)
            healthBarFill.color = Color.green;
        else if (healthPercent > 0.3f)
            healthBarFill.color = Color.yellow;
        else
            healthBarFill.color = Color.red;

        // Обновляем текст
        if (healthText != null)
        {
            healthText.text = $"{figure.currentHealth}/{figure.maxHealth}";
        }
    }

    public void UpdateActionBar(float actionValue)
    {
        if (actionBarFill == null) return;

        actionBarFill.fillAmount = actionValue / 100f;

        // Меняем цвет когда готов к действию
        if (actionValue >= 100f)
            actionBarFill.color = Color.cyan;
        else
            actionBarFill.color = new Color(0.5f, 0.5f, 1f);
    }

    void OnMouseEnter()
    {
        if (G.battleSystem != null && figure != null && figure.IsAlive())
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
        if (G.battleSystem != null && figure != null && figure.IsAlive())
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
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

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

        // Скрываем полосы
        if (healthBarContainer != null) healthBarContainer.SetActive(false);
        if (actionBarContainer != null) actionBarContainer.SetActive(false);

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

    // Показываем эффект баффа
    public void ShowBuffEffect()
    {
        if (buffEffectPrefab != null && effectsContainer != null)
        {
            GameObject effect = Instantiate(buffEffectPrefab, effectsContainer);
            StartCoroutine(DestroyEffectAfterDelay(effect, 2f));
        }
        else
        {
            // Альтернативный визуал если нет префаба
            StartCoroutine(BuffGlow(Color.green));
        }
    }

    // Показываем эффект дебаффа
    public void ShowDebuffEffect()
    {
        if (debuffEffectPrefab != null && effectsContainer != null)
        {
            GameObject effect = Instantiate(debuffEffectPrefab, effectsContainer);
            StartCoroutine(DestroyEffectAfterDelay(effect, 2f));
        }
        else
        {
            // Альтернативный визуал если нет префаба
            StartCoroutine(BuffGlow(Color.magenta));
        }
    }

    IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect);
    }

    // Эффект свечения для баффов/дебаффов
    IEnumerator BuffGlow(Color glowColor)
    {
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        float duration = 0.5f;
        float elapsed = 0f;

        // Плавное свечение
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 4f, 1f);
            spriteRenderer.color = Color.Lerp(original, glowColor, t * 0.5f);
            yield return null;
        }

        spriteRenderer.color = original;
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
        textMesh.characterSize = 0.1f;

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