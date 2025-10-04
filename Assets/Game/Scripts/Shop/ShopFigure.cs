using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopFigure : MonoBehaviour
{
    [SerializeField] private ShelfFigure shelfFigurePrefab;

    void Awake()
    {
        G.shopFigures = this;
    }

    public List<Figure> AvailableFiguresToBuy(int lvl)
    {
        return FigureFactory.GetFiguresForLevel(lvl);
    }

    public List<Figure> AvailableFiguresToSwap(int lvl)
    {
        return FigureFactory.GetFiguresForLevel(lvl);
    }

    public void BuyFigure(Figure figure)
    {
        if (figure != null && figure.cost <= G.playerData.money)
        {
            var shelfFigure = Instantiate(shelfFigurePrefab);
            shelfFigure.data = FigureFactory.Clone(figure);

            G.shelfManager.AddFigureToFirstEmpty(shelfFigure);
            G.playerData.money -= figure.cost;
        }
    }
    public void BuyRandomFigure()
    {
        Debug.Log("BuyRandomFigure");

            var figure = FigureFactory.GetRandomFigure(G.playerData.level);
        Debug.Log("BuyRandomFigure" + figure);
        if (figure != null) BuyFigure(figure);
    }

    public void SellFigure(Figure figure)
    {
        if (figure != null)
        {
            G.playerData.money += figure.cost;
            G.shelfManager.RemoveFigure(figure);
        }
    }
    public void SwapFigures(Figure toAdd, Figure toRemove)
    {
        if (toAdd != null && toRemove != null)
        {
            var shelfFigure = Instantiate(shelfFigurePrefab);
            shelfFigure.data = FigureFactory.Clone(toAdd);
            G.shelfManager.AddFigureToSlot(shelfFigure, G.shelfManager.GetSlotIndexOfFigure(toRemove));
            G.shelfManager.RemoveFigure(toRemove);
        }
    }
}