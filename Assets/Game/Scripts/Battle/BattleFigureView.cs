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
  $"<b><color=#8B4513>{data.name}</color></b> " +
  $"<size=80%><color=#4682B4> [Lvl {data.lvl}]</color></size> \n" +
  $"<i><color=#444444>{data.description}</color></i>\n\n" +
  $"<b><color=#228B22>Health:</color></b> {data.currentHealth}/{data.maxHealth}\n" +
  $"<b><color=#B22222>Damage:</color></b> {data.damage}\n" +
  $"<b><color=#1E90FF>Speed:</color></b> {data.speed}\n" +
  $"<b><color=#696969>Defense:</color></b> {data.defense}\n" +
  $"<b><color=#DAA520>Cost:</color></b> {data.cost}";
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

    public IEnumerator RangedAttackAnimation(Vector3 targetPosition, Skill skill = null)
    {
        Debug.Log($"RangedAttackAnimation вызвана для {FigureId}, skill: {(skill != null ? skill.skillName : "null")}");

        // Определяем цвет и параметры снаряда в зависимости от скилла
        Color projectileColor = new Color(1f, 0.5f, 0f, 0.9f); // Оранжевый по умолчанию
        float projectileSize = 0.3f;

        if (skill != null)
        {
            if (skill.skillName.Contains("Снаряд"))
            {
                projectileColor = new Color(0.7f, 0.3f, 1f, 1f);
                projectileSize = 0.3f;
            }
            else
            {
                projectileColor = new Color(0.8f, 0.6f, 0.2f, 1f);
                projectileSize = 0.25f;
            }
        }

        // Создаём снаряд
        GameObject projectile = new GameObject("Projectile");
        projectile.transform.position = transform.position;

        SpriteRenderer projRenderer = projectile.AddComponent<SpriteRenderer>();
        projRenderer.sprite = CreateCircleSprite();
        projRenderer.color = projectileColor;
        projRenderer.sortingOrder = spriteRenderer.sortingOrder + 2;

        projectile.transform.localScale = Vector3.one * projectileSize;

        // Добавляем след
        TrailRenderer trail = projectile.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.2f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(projectileColor.r, projectileColor.g, projectileColor.b, 0.8f);
        trail.endColor = new Color(projectileColor.r, projectileColor.g, projectileColor.b, 0f);

        // Летим к цели
        float duration = 0.4f;
        float elapsed = 0f;
        Vector3 startPos = projectile.transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Параболическая траектория
            Vector3 currentPos = Vector3.Lerp(startPos, targetPosition, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * 0.5f;

            projectile.transform.position = currentPos;

            // Вращение снаряда
            projectile.transform.Rotate(0, 0, 720f * Time.deltaTime);

            yield return null;
        }

        // Эффект попадания
        StartCoroutine(ProjectileImpactEffect(targetPosition, projectileColor));

        Destroy(projectile);
    }

    IEnumerator ProjectileImpactEffect(Vector3 position, Color impactColor)
    {
        // Создаём взрыв из частиц
        int particleCount = 12;
        GameObject[] particles = new GameObject[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("ImpactParticle");
            particle.transform.position = position;

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = impactColor;
            sr.sortingOrder = spriteRenderer.sortingOrder + 3;

            particle.transform.localScale = Vector3.one * Random.Range(0.1f, 0.15f);
            particles[i] = particle;
        }

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < particleCount; i++)
            {
                if (particles[i] != null)
                {
                    float angle = (360f / particleCount) * i;
                    Vector3 direction = new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad),
                        0
                    );

                    particles[i].transform.position = position + direction * t * 0.5f;

                    SpriteRenderer sr = particles[i].GetComponent<SpriteRenderer>();
                    Color col = sr.color;
                    col.a = Mathf.Lerp(1f, 0f, t);
                    sr.color = col;

                    float scale = Mathf.Lerp(1f, 0.3f, t);
                    particles[i].transform.localScale = Vector3.one * Random.Range(0.1f, 0.15f) * scale;
                }
            }

            yield return null;
        }

        foreach (var particle in particles)
        {
            if (particle != null) Destroy(particle);
        }
    }

    public void PlayHitAnimation()
    {
        if (figure != null)
        {
            StartCoroutine(HitFlash());
            StartCoroutine(Shake());
        }
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

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
        if (spriteRenderer == null) yield break;

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

        if (healthBarContainer != null) healthBarContainer.SetActive(false);
        if (actionBarContainer != null) actionBarContainer.SetActive(false);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(original.r, original.g, original.b, alpha);

            float scale = Mathf.Lerp(1f, 0.5f, elapsed / duration);
            transform.localScale = originalScale * scale;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void ShowBuffEffect()
    {
        Debug.Log($"ShowBuffEffect вызван для {FigureId}");
        StartCoroutine(BuffEffect());
    }

    public void ShowDebuffEffect()
    {
        Debug.Log($"ShowDebuffEffect вызван для {FigureId}");
        StartCoroutine(DebuffEffect());
    }

    public void ShowHealEffect()
    {
        Debug.Log($"ShowHealEffect вызван для {FigureId}");
        StartCoroutine(HealParticles());
    }

    IEnumerator BuffEffect()
    {
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        Vector3 originalScale = transform.localScale;
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float pulse = Mathf.Sin(t * Mathf.PI * 3f);

            spriteRenderer.color = Color.Lerp(original, new Color(1f, 0.84f, 0f), pulse * 0.5f);
            transform.localScale = originalScale * (1f + pulse * 0.1f);

            yield return null;
        }

        spriteRenderer.color = original;
        transform.localScale = originalScale;
    }

    IEnumerator DebuffEffect()
    {
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        Vector3 originalScale = transform.localScale;
        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float pulse = Mathf.Sin(t * Mathf.PI * 3f);

            spriteRenderer.color = Color.Lerp(original, new Color(0.8f, 0f, 0.8f), pulse * 0.5f);
            transform.localScale = originalScale * (1f - pulse * 0.08f);

            yield return null;
        }

        spriteRenderer.color = original;
        transform.localScale = originalScale;
    }

    IEnumerator HealParticles()
    {
        int particleCount = 8;
        GameObject[] particles = new GameObject[particleCount];

        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("HealParticle");
            particle.transform.position = transform.position + new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.2f, 0.2f),
                0
            );

            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite();
            sr.color = new Color(0.2f, 1f, 0.3f, 0.8f);
            sr.sortingOrder = spriteRenderer.sortingOrder + 1;

            particle.transform.localScale = Vector3.one * Random.Range(0.1f, 0.2f);
            particles[i] = particle;
        }

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            for (int i = 0; i < particleCount; i++)
            {
                if (particles[i] != null)
                {
                    Vector3 pos = particles[i].transform.position;
                    pos.y += Time.deltaTime * Random.Range(0.8f, 1.5f);
                    particles[i].transform.position = pos;

                    SpriteRenderer sr = particles[i].GetComponent<SpriteRenderer>();
                    Color col = sr.color;
                    col.a = Mathf.Lerp(0.8f, 0f, t);
                    sr.color = col;
                }
            }

            yield return null;
        }

        foreach (var particle in particles)
        {
            if (particle != null) Destroy(particle);
        }
    }

    Sprite CreateCircleSprite()
    {
        int size = 16;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        int center = size / 2;
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                pixels[y * size + x] = distance < radius ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect);
    }

    public void ShowDamageText(int amount, bool isHeal = false)
    {
        StartCoroutine(FloatingText(amount.ToString(), isHeal ? Color.green : Color.red));
    }

    IEnumerator FloatingText(string text, Color color)
    {
        GameObject textObj = new GameObject("DamageText");
        textObj.transform.position = transform.position + Vector3.up * 0.5f;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 20;
        textMesh.color = color;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.1f;

        float duration = 0.5f;
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