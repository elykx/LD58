using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShelfFigure : MonoBehaviour
{
    public Figure data;
    private Vector3 offset;
    private Camera cam;

    [HideInInspector] public ShelfSlot currentSlot;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        if (currentSlot != null)
            currentSlot.Clear(); // освободить слот
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        // ищем ближайший слот
        ShelfSlot nearest = FindNearestSlot();
        if (nearest != null && nearest.IsEmpty)
        {
            nearest.PlaceFigure(this);
        }
        else if (currentSlot != null)
        {
            // если слот был занят — вернем обратно
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
        float radius = 0.5f; // радиус поиска
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            ShelfSlot slot = hit.GetComponent<ShelfSlot>();
            if (slot != null)
                return slot;
        }

        return null;
    }
}