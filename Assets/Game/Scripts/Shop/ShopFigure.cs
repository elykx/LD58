using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopFigure : MonoBehaviour
{
    public List<ShopSlot> shopSlots;
    public SellArea sellArea;

    void Awake()
    {
        G.shopFigures = this;
        HideSellArea();
    }

    public List<Figure> AvailableFiguresToBuy(int lvl)
    {
        return FigureGenerator.GetFiguresForLevel(lvl);
    }

    public List<Figure> AvailableFiguresToSwap(int lvl)
    {
        return FigureGenerator.GetFiguresForLevel(lvl);
    }

    public void BuyFigure(Figure figure)
    {
        if (figure != null && figure.cost <= G.playerData.money)
        {
            // отправляем сразу на Shelf
            G.shelfManager.AddFigureToFirstEmpty(figure.id);

            // снимаем деньги
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

    public void SellFigure(string figureId)
    {
        var figure = G.figureManager.GetFigure(figureId);

        if (figure != null)
        {
            G.playerData.money += figure.cost;
            G.shelfManager.RemoveFigure(figure);
        }
    }
    // public void SwapFigures(Figure toAdd, Figure toRemove)
    // {
    //     if (toAdd != null && toRemove != null)
    //     {
    //         var shelfFigure = Instantiate(viewShopFIgurePrefab);
    //         shelfFigure.data = FigureFactory.Clone(toAdd);
    //         G.shelfManager.AddFigureToSlot(shelfFigure, G.shelfManager.GetSlotIndexOfFigure(toRemove));
    //         G.shelfManager.RemoveFigure(toRemove);
    //     }
    // }

    public void GenerateShopFigures(int level)
    {
        ClearShopFigures();

        var available = AvailableFiguresToBuy(level);
        for (int i = 0; i < shopSlots.Count && i < available.Count; i++)
        {
            G.figureManager.AddFigure(available[i]);
            var go = Instantiate(G.figureManager.viewShopFigurePrefab, shopSlots[i].spawnPoint, false);
            var view = go.GetComponent<ViewShopFigure>();
            view.FigureId = available[i].id;

            shopSlots[i].currentFigure = view;
        }
    }

    private void ClearShopFigures()
    {
        foreach (var slot in shopSlots)
        {
            if (slot.currentFigure != null)
            {
                Destroy(slot.currentFigure.gameObject);
                slot.currentFigure = null;
            }
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

}