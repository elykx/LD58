using LitMotion;
using UnityEngine;

public class ViewShopFigure : MonoBehaviour
{
    public string FigureId;
    public SpriteRenderer figureSprite;

    private Vector3 offset;
    private TooltipShower tooltip;
    private bool isDragging = false;
    private Vector3 originalPos;
    private float fixedY;

    private MotionHandle swayHandle;
    private float baseSway = 10f;
    private float dragSway = 25f;
    private float swayDuration = 1f;
    public Material melee;
    public Material ranged;
    public Material mage;
    public Material support;
    public Material unique;
    public SpriteRenderer boxSprite;


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Dragging");
    }


    void Start()
    {
        var fig = G.figureManager.GetFigure(FigureId);
        figureSprite.sprite = fig.sprite;
        Material boxMat = boxSprite.material;
        switch (fig.figureClass)
        {
            case FigureClass.Melee:
                boxMat = melee;
                break;
            case FigureClass.Ranged:
                boxMat = ranged;
                break;
            case FigureClass.Mage:
                boxMat = mage;
                break;
            case FigureClass.Support:
                boxMat = support;
                break;
            case FigureClass.Unique:
                boxMat = unique;
                break;
        }

        boxSprite.material = boxMat;

        originalPos = transform.position;
        tooltip = GetComponent<TooltipShower>();
        UpdateTooltip();

        StartSway(baseSway);
    }

    private void UpdateTooltip()
    {
        Figure data = G.figureManager.GetFigure(FigureId);
        if (tooltip != null)
        {
            // Заголовок: имя + уровень
            tooltip.tooltipText =
    $"<b><color=#8B4513>{data.name}</color></b> " +
    $"<size=80%><color=#4682B4>[Lvl {data.lvl}]</color></size>" +
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

        StartSway(dragSway);


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

        StartSway(baseSway);

        BuyArea nearest = FindNearestBuyArea();
        Debug.Log("nearest: " + nearest);
        if (nearest != null)
        {
            Figure data = G.figureManager.GetFigure(FigureId);
            if (data != null && G.playerData.money >= data.cost && !G.shelfManager.CheckFull())
            {
                G.shopFigures.BuyFigure(data);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("monet dasdsaasd");
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

    private void StartSway(float amplitude)
    {
        if (swayHandle.IsActive())
        {
            swayHandle.Cancel();
        }

        swayHandle = LMotion.Create(-amplitude, amplitude, swayDuration)
            .WithLoops(-1, LoopType.Yoyo)
            .Bind(rotZ =>
            {
                transform.localRotation = Quaternion.Euler(0, 0, rotZ);
            })
            .AddTo(gameObject);
    }
}