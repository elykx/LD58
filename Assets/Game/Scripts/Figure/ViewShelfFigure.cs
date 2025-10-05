using LitMotion;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ViewShelfFigure : MonoBehaviour
{
    public string FigureId;
    public ShelfSlot currentSlot;
    private Vector3 offset;
    public SpriteRenderer figureSprite;


    private TooltipShower tooltip;
    private bool isDragging = false;

    private MotionHandle idleMotionHandle;
    private MotionHandle dragMotionHandle;
    private Vector3 originalScale;
    private float nextIdleTime;
    private bool isBeingDestroyed = false;


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Dragging");
        originalScale = transform.localScale;
    }

    void Start()
    {
        Figure fig = G.figureManager.GetFigure(FigureId);
        figureSprite.sprite = fig.sprite;
        tooltip = GetComponent<TooltipShower>();
        UpdateTooltip(fig);

        ScheduleNextIdleAnimation();
    }

    private void OnDestroy()
    {
        if (idleMotionHandle.IsActive()) idleMotionHandle.Cancel();
        if (dragMotionHandle.IsActive()) dragMotionHandle.Cancel();
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

    private void Update()
    {
        var figureInDb = G.figureManager.GetFigure(FigureId);
        if (figureInDb == null || figureInDb.currentHealth <= 0)
        {
            if (isBeingDestroyed) return;
            Debug.Log($"Figure {FigureId} is dead / REMOVE");
            G.shelfManager.RemoveFigure(FigureId);
            isBeingDestroyed = true; // <-- УСТАНОВИТЬ ФЛАГ
            Destroy(gameObject);
            return; // <-- ВЫЙТИ ИЗ UPDATE
        }
        UpdateTooltip(figureInDb);

        if (!isDragging && Time.time >= nextIdleTime)
        {
            PlayRandomIdleAnimation();
            ScheduleNextIdleAnimation();
        }

        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            if (IsMouseOverThis())
            {
                StartDrag();
                G.shelfManager.ShowSlotsSprites();
                G.shelfManager.ShowSellArea();
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            DragUpdate();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
            G.shelfManager.HideSlotsSprites();
            G.shelfManager.HideSellArea();
        }
    }

    private void ScheduleNextIdleAnimation()
    {
        nextIdleTime = Time.time + Random.Range(1f, 12f);
    }

    private void PlayRandomIdleAnimation()
    {
        if (idleMotionHandle.IsActive()) idleMotionHandle.Cancel();

        int animType = Random.Range(0, 4);

        switch (animType)
        {
            case 0: // Подпрыгивание
                Vector3 startPos = transform.localPosition;
                idleMotionHandle = LMotion.Create(0f, 0.2f, 0.3f)
                    .WithEase(Ease.OutQuad)
                    .WithLoops(2, LoopType.Yoyo)
                    .Bind(offset =>
                    {
                        Vector3 pos = startPos;
                        pos.y += offset;
                        transform.localPosition = pos;
                    })
                    .AddTo(gameObject);
                break;

            case 1: // Покачивание
                idleMotionHandle = LMotion.Create(-5f, 5f, 0.4f)
                    .WithEase(Ease.InOutSine)
                    .WithLoops(2, LoopType.Yoyo)
                    .Bind(angle =>
                    {
                        transform.localRotation = Quaternion.Euler(0, 0, angle);
                    })
                    .AddTo(gameObject);
                break;

            case 2: // Увеличение (breathe)
                idleMotionHandle = LMotion.Create(1f, 1.1f, 0.5f)
                    .WithEase(Ease.InOutSine)
                    .WithLoops(2, LoopType.Yoyo)
                    .Bind(scale =>
                    {
                        transform.localScale = originalScale * scale;
                    })
                    .AddTo(gameObject);
                break;

            case 3: // Быстрое встряхивание
                idleMotionHandle = LMotion.Create(-10f, 10f, 0.1f)
                    .WithEase(Ease.Linear)
                    .WithLoops(4, LoopType.Yoyo)
                    .Bind(angle =>
                    {
                        transform.localRotation = Quaternion.Euler(0, 0, angle);
                    })
                    .AddTo(gameObject);
                break;
        }
    }

    private bool IsMouseOverThis()
    {
        Vector2 mousePos = GetMouseWorldPos();

        int figureMask = 1 << gameObject.layer;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, figureMask);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    private void StartDrag()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();

        if (idleMotionHandle.IsActive()) idleMotionHandle.Cancel();

        transform.localRotation = Quaternion.identity;
        transform.localScale = originalScale;

        dragMotionHandle = LMotion.Create(-8f, 8f, 0.5f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(angle =>
            {
                transform.localRotation = Quaternion.Euler(0, 0, angle);
            })
            .AddTo(gameObject);

        if (currentSlot != null)
            currentSlot.Clear();
    }

    private void DragUpdate()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private void EndDrag()
    {
        isDragging = false;

        if (dragMotionHandle.IsActive()) dragMotionHandle.Cancel();
        transform.localRotation = Quaternion.identity;

        ShelfSlot nearest = FindNearestSlot();

        // Проверяем что слот найден И (пустой ИЛИ это наш текущий слот)
        if (nearest != null && (nearest.IsEmpty || nearest == currentSlot))
        {
            nearest.PlaceFigure(this);
        }
        else
        {
            // Проверяем зону продажи
            SellArea sellZone = FindSellZone();
            if (sellZone != null)
            {
                Debug.Log("Selling figure " + FigureId);
                G.shopFigures.SellFigure(FigureId);
                Debug.Log($"Figure {FigureId} is SELL / REMOVE");
                G.shelfManager.RemoveFigure(FigureId);
                isBeingDestroyed = true; // <-- УСТАНОВИТЬ ФЛАГ
                Destroy(gameObject);
                return; // <-- ВЫЙТИ ИЗ UPDATE
            }
            else if (currentSlot != null)
            {
                // Возвращаем в текущий слот
                currentSlot.PlaceFigure(this);
            }
        }

        ScheduleNextIdleAnimation();
    }

    private Vector3 GetMouseWorldPos()
    {
        Camera camera = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -camera.transform.position.z;
        return camera.ScreenToWorldPoint(mousePos);
    }

    private ShelfSlot FindNearestSlot()
    {
        float maxDistance = 0.5f;
        int slotMask = LayerMask.GetMask("ShellPos");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxDistance, slotMask);

        ShelfSlot nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            ShelfSlot slot = hit.GetComponent<ShelfSlot>();
            if (slot != null)
            {
                float dist = Vector2.Distance(transform.position, slot.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = slot;
                }
            }
        }

        if (nearest != null && minDist > maxDistance)
        {
            return null;
        }

        return nearest;
    }

    private SellArea FindSellZone()
    {
        float radius = 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        SellArea nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            SellArea slot = hit.GetComponent<SellArea>();
            if (slot != null)
            {
                float dist = Vector2.Distance(transform.position, slot.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = slot;
                }
            }
        }

        return nearest;
    }

    internal void AddLvl()
    {
        G.figureManager.AddLvl(FigureId);
    }
}