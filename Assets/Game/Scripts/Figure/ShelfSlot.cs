// Класс ячейки шкафа
using System;
using UnityEngine;

public class ShelfSlot : MonoBehaviour
{
    [HideInInspector] public ViewShelfFigure currentFigure;
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