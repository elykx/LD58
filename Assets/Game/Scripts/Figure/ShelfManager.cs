using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [Header("Слоты шкафа")]
    [SerializeField] private List<ShelfSlot> slots;
    public ShelfFigure baseFigure;

    void Awake()
    {
        G.shelfManager = this;
    }

    public bool AddFigureToSlot(ShelfFigure figure, int slotIndex)
    {
        ShelfFigure newFigure = Instantiate(figure);
        newFigure.gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        ShelfSlot slot = slots[slotIndex];
        if (!slot.IsEmpty) return false;

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

    public bool AddFigureToFirstEmpty(ShelfFigure figure)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.PlaceFigure(figure);
                return true;
            }
        }
        return false;
    }

    public int GetSlotIndex(ShelfSlot slot)
    {
        return slots.IndexOf(slot);
    }

    public ShelfFigure GetFigureFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return null;
        return slots[slotIndex].currentFigure;
    }

    public int GetSlotIndexOfFigure(Figure figure)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure != null && slots[i].currentFigure.data.id == figure.id)
                return i;
        }
        return -1;
    }

    public void RemoveFigure(Figure figure)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentFigure != null && slots[i].currentFigure.data.id == figure.id)
                RemoveFigureFromSlot(i);
        }
    }
}
