using System.Linq;
using GameAnalyticsSDK;
using Runtime;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        G.main = this;
    }

    private void Start()
    {
        GameAnalytics.Initialize();
        CMS.Init();
        UIDebug.Log("Игра запущена.", Color.aquamarine);
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 5);
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 12);
        var fig = G.shelfManager.GetFigureFromSlot(5);
        if (fig != null)
        {
            UIDebug.Log(fig.data.name, Color.softRed);
            fig.data.name = "test12312";
            var fig2 = G.shelfManager.GetFigureFromSlot(12);
            UIDebug.Log(fig2.data.name, Color.softRed);
            UIDebug.Log(fig.data.name, Color.softRed);

        }
    }
}
