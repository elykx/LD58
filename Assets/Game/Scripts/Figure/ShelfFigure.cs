using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShelfFigure : MonoBehaviour
{
    public Figure data;
    private Vector3 offset;
    private Camera cam;

    [HideInInspector] public ShelfSlot currentSlot;

    private TooltipTrigger tooltip;
    private bool isDragging = false;
    private int originalLayer;

    private void Awake()
    {
        cam = Camera.main;
        gameObject.layer = LayerMask.NameToLayer("Dragging");
    }

    void Start()
    {
        tooltip = GetComponent<TooltipTrigger>();
    }

    private void UpdateTooltip()
    {
        if (tooltip != null)
        {
            // Заголовок: имя + уровень
            tooltip.tooltipTitle =
                $"<b><color=#FFD700>{data.name}</color></b> " +
                $"<size=80%><color=#00BFFF>[Lvl {data.lvl}]</color></size>";

            // Тело: описание и статы
            tooltip.tooltipContent =
                $"<i>{data.description}</i>\n\n" + // можно заменить на свой description
                $"<b><color=#32CD32>Здоровье:</color></b> {data.currentHealth}/{data.maxHealth}\n" +
                $"<b><color=#DC143C>Урон:</color></b> {data.damage}\n" +
                $"<b><color=#1E90FF>Скорость:</color></b> {data.speed}\n" +
                $"<b><color=#A9A9A9>Защита:</color></b> {data.defense}\n" +
                $"<b><color=#FFD700>Стоимость:</color></b> {data.cost}";
        }
    }

    private void Update()
    {
        // Начало драга
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            if (IsMouseOverThis())
            {
                UpdateTooltip();
                StartDrag();
                G.shelfManager.ShowSlotsSprites();
            }
        }

        // Процесс драга
        if (isDragging && Input.GetMouseButton(0))
        {
            DragUpdate();
        }

        // Конец драга
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
            G.shelfManager.HideSlotsSprites();
        }
    }

    private bool IsMouseOverThis()
    {
        Vector2 mousePos = GetMouseWorldPos();

        // Кастим луч только по слою фигурок
        int figureMask = 1 << gameObject.layer;
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, figureMask);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

    private void StartDrag()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();

        if (currentSlot != null)
            currentSlot.Clear();

        // Запоминаем слой и переносим в "Dragging"
        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dragging");
    }

    private void DragUpdate()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private void EndDrag()
    {
        isDragging = false;

        // Возвращаем исходный слой
        gameObject.layer = originalLayer;

        // Ищем ближайший слот
        ShelfSlot nearest = FindNearestSlot();
        if (nearest != null && nearest.IsEmpty)
        {
            nearest.PlaceFigure(this);
        }
        else if (currentSlot != null)
        {
            currentSlot.PlaceFigure(this);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -cam.transform.position.z;
        return cam.ScreenToWorldPoint(mousePos);
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
}