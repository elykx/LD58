using System;
using System.Collections.Generic;
using UnityEngine;

public class FigureManager : MonoBehaviour
{
    public List<Figure> figures = new List<Figure>(); // TODO: УДАЛЯТЬ УБИТЫХ
    public ViewShelfFigure viewShelfFigurePrefab;
    public ViewShopFigure viewShopFigurePrefab;



    void Awake()
    {
        G.figureManager = this;
        FigureConsts.Init();
    }

    public Figure GetFigure(string id)
    {
        return figures.Find(f => f.id == id);
    }

    public void AddFigure(Figure figure)
    {
        figures.Add(figure);
    }

    public void RemoveFigureById(string id)
    {
        figures.RemoveAll(f => f.id == id);
    }

    internal void AddLvl(string figureId)
    {
        var figure = GetFigure(figureId);
        figure.AddLvl();
    }
}