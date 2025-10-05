using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [Header("Слоты шкафа")]
    [SerializeField] private List<ShelfSlot> slots;
    public SellArea sellArea;
    public int currentNumFiguresInShelf = 0;


    void Awake()
    {
        G.shelfManager = this;
    }

    public bool AddFigureToSlot(string figureId, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        ShelfSlot slot = slots[slotIndex];
        if (!slot.IsEmpty) return false;

        var prefab = G.figureManager.viewShelfFigurePrefab;
        var newFigure = Instantiate(prefab);
        newFigure.FigureId = figureId;
        newFigure.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        slot.PlaceFigure(newFigure);
        currentNumFiguresInShelf++;
        return true;
    }



    public bool AddFigureToFirstEmpty(string figureId)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                var prefab = G.figureManager.viewShelfFigurePrefab;
                var newFigure = Instantiate(prefab);
                newFigure.FigureId = figureId;
                newFigure.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                currentNumFiguresInShelf++;
                slot.PlaceFigure(newFigure);
                return true;
            }
        }
        return false;
    }

    public int GetSlotIndex(ShelfSlot slot)
    {
        return slots.IndexOf(slot);
    }

    public ShelfSlot GetSlotFromFigure(string figureId)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure == null) continue;
            if (slots[i].currentFigure.FigureId == figureId)
            {
                return slots[i];
            }
        }
        return null;
    }
    public ViewShelfFigure GetFigureFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return null;
        return slots[slotIndex].currentFigure;
    }

    public void RemoveFigure(string figureId)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure != null && slots[i].currentFigure.FigureId == figureId)
            {
                RemoveFigureFromSlot(i);
                return;
            }
        }
    }

    public bool RemoveFigureFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        ShelfSlot slot = slots[slotIndex];
        if (slot.IsEmpty) return false;

        slot.Clear();
        currentNumFiguresInShelf--;
        return true;
    }

    public void ShowSlotsSprites()
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.ShowSprite();
            }
        }
    }

    public void HideSlotsSprites()
    {
        foreach (var slot in slots)
        {
            slot.HideSprite();
        }
    }

    public void ShowSellArea()
    {
        sellArea.gameObject.SetActive(true);
    }

    public void HideSellArea()
    {
        sellArea.gameObject.SetActive(false);
    }

    public bool CheckFull()
    {
        if (currentNumFiguresInShelf >= slots.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
