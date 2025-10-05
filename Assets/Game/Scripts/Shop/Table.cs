using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject hoverSprite;
    public TooltipShower tooltipShower;
    public Collider2D col;

    private void Start()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);

        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (G.playerData.current_pos != "main")
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
        }
    }

    private void OnMouseEnter()
    {
        if (G.playerData.current_pos != "main")
            return;
        if (hoverSprite != null)
            hoverSprite.SetActive(true);

        if (G.playerData.shopAlreadyOpened)
        {
            tooltipShower.tooltipText = "Already been visited";
        }
        else
        {
            tooltipShower.tooltipText = "Visit shop";
        }
    }

    private void OnMouseExit()
    {
        if (hoverSprite != null)
            hoverSprite.SetActive(false);
    }

    private void OnMouseDown()
    {
        G.main.GoShop();
    }
}