// Класс ячейки шкафа
using System;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    public ViewShelfFigure currentFigure;
    public bool IsEmpty => currentFigure == null;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void ShowSprite()
    {
        spriteRenderer.enabled = true;
    }

    public void HideSprite()
    {
        spriteRenderer.enabled = false;
    }

    public void PlaceFigure(ViewShelfFigure figure)
    {
        Debug.Log($"PlaceFigure called for slot, IsEmpty: {IsEmpty}, figure: {figure.FigureId}");

        // Если слот занят - не размещаем
        if (!IsEmpty)
        {
            Debug.Log("Slot is already occupied!");
            return;
        }

        // Получаем старый слот фигуры
        var oldSlot = figure.currentSlot;

        // СНАЧАЛА очищаем старый слот
        if (oldSlot != null && oldSlot != this)
        {
            Debug.Log($"Clearing old slot");
            oldSlot.Clear();
        }

        // ПОТОМ устанавливаем в новый слот
        currentFigure = figure;
        figure.currentSlot = this;
        figure.transform.position = transform.position;

        Debug.Log($"Figure {figure.FigureId} placed successfully");
    }

    public void Clear()
    {
        currentFigure = null;
    }
}