using LitMotion;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ViewShelfFigure : MonoBehaviour
{
    public string FigureId;
    [HideInInspector] public ShelfSlot currentSlot;
    private Vector3 offset;
    public SpriteRenderer figureSprite;


    private TooltipShower tooltip;
    private bool isDragging = false;

    private MotionHandle idleMotionHandle;
    private MotionHandle dragMotionHandle;
    private Vector3 originalScale;
    private float nextIdleTime;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Dragging");
        originalScale = transform.localScale;
    }

    void Start()
    {
        figureSprite.sprite = G.figureManager.GetFigure(FigureId).sprite;
        tooltip = GetComponent<TooltipShower>();
        UpdateTooltip();

        ScheduleNextIdleAnimation();
    }

    private void OnDestroy()
    {
        if (idleMotionHandle.IsActive()) idleMotionHandle.Cancel();
        if (dragMotionHandle.IsActive()) dragMotionHandle.Cancel();
    }

    private void UpdateTooltip()
    {
        Figure data = G.figureManager.GetFigure(FigureId);
        if (tooltip != null)
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

    private void Update()
    {

        if (!isDragging && Time.time >= nextIdleTime)
        {
            PlayRandomIdleAnimation();
            ScheduleNextIdleAnimation();
        }

        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            if (IsMouseOverThis())
            {
                UpdateTooltip();
                StartDrag();
                G.shelfManager.ShowSlotsSprites();
                G.shopFigures.ShowSellArea();
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
            G.shopFigures.HideSellArea();
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

        // Останавливаем покачивание при драге
        if (dragMotionHandle.IsActive()) dragMotionHandle.Cancel();

        // Плавно возвращаем rotation к нулю
        transform.localRotation = Quaternion.identity;

        ShelfSlot nearest = FindNearestSlot();
        if (nearest != null && nearest.IsEmpty)
        {
            nearest.PlaceFigure(this);
        }
        else if (nearest == null)
        {
            SellArea sellZone = FindSellZone();
            if (sellZone != null)
            {
                G.shopFigures.SellFigure(FigureId);
                Destroy(gameObject);
            }
        }
        else if (currentSlot != null)
        {
            currentSlot.PlaceFigure(this);
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
        float radius = 0.5f;
        int slotMask = LayerMask.GetMask("ShellPos");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, slotMask);

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
}