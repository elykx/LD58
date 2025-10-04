using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [Header("Слоты шкафа")]
    [SerializeField] private List<ShelfSlot> slots;

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
        newFigure.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        slot.PlaceFigure(newFigure);
        return true;
    }

    public bool RemoveFigureFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        ShelfSlot slot = slots[slotIndex];
        if (slot.IsEmpty) return false;

        slot.Clear();
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
                newFigure.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

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

    public ViewShelfFigure GetFigureFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return null;
        return slots[slotIndex].currentFigure;
    }

    public int GetSlotIndexOfFigure(Figure figure)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure != null && slots[i].currentFigure.FigureId == figure.id)
                return i;
        }
        return -1;
    }

    public void RemoveFigure(Figure figure)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure != null && slots[i].currentFigure.FigureId == figure.id)
                RemoveFigureFromSlot(i);
        }
    }

    public void ShowSlotsSprites()
    {
        foreach (var slot in slots)
        {
            if (slot.currentFigure == null)
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
}
