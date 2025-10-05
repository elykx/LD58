using UnityEngine;

public class ViewShopFigure : MonoBehaviour
{
    public string FigureId;


    private Vector3 offset;
    private TooltipShower tooltip;
    private bool isDragging = false;
    private Vector3 originalPos;
    private float fixedY;


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Dragging");
    }


    void Start()
    {
        originalPos = transform.position;
        tooltip = GetComponent<TooltipShower>();
        UpdateTooltip();
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
            $"<b><color=#FFD700>Стоимость:</color></b> {data.cost}";
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            if (IsMouseOverThis())
            {
                StartDrag();
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            DragUpdate();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
    }

    private bool IsMouseOverThis()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    private void StartDrag()
    {
        isDragging = true;
        fixedY = transform.position.y;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

        float enter;
        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }
    }

    private void DragUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

        float enter;
        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = new Vector3(hitPoint.x + offset.x, fixedY, hitPoint.z + offset.z);
        }
    }

    private void EndDrag()
    {
        isDragging = false;

        BuyArea nearest = FindNearestBuyArea();
        Debug.Log("nearest: " + nearest);
        if (nearest != null)
        {
            Figure data = G.figureManager.GetFigure(FigureId);
            Debug.Log(data);

            if (data != null && G.playerData.money >= data.cost)
            {
                G.shopFigures.BuyFigure(data);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("monet dasdsaasd");

                // Вернуть обратно на место, если не хватает денег
                ReturnToShopPosition();
            }
        }
        else
        {
            // если не попали в зону — вернуть обратно
            ReturnToShopPosition();
        }
    }

    private void ReturnToShopPosition()
    {
        transform.position = originalPos;
    }

    private BuyArea FindNearestBuyArea()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 10f, Vector3.down); // сверху вниз
        RaycastHit[] hits = Physics.RaycastAll(ray, 20f); // длина 20 юнитов

        BuyArea nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            BuyArea area = hit.collider.GetComponent<BuyArea>();
            if (area != null)
            {
                // расстояние только по XZ
                Vector2 objXZ = new Vector2(transform.position.x, transform.position.z);
                Vector2 areaXZ = new Vector2(area.transform.position.x, area.transform.position.z);

                float dist = Vector2.Distance(objXZ, areaXZ);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = area;
                }
            }
        }

        return nearest;
    }

}