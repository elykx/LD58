// Класс ячейки шкафа
using System;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    [HideInInspector] public ShelfFigure currentFigure;
    public bool IsEmpty => currentFigure == null;

    public void PlaceFigure(ShelfFigure figure)
    {
        if (!IsEmpty) return;

        currentFigure = figure;
        figure.transform.position = transform.position;
        figure.currentSlot = this;
    }

    public void Clear()
    {
        currentFigure = null;
    }
}