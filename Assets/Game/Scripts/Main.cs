using System.Collections.Generic;
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
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 1);
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 3);
        var fig = G.shelfManager.GetFigureFromSlot(5);
        if (fig != null)
        {
            UIDebug.Log(fig.data.name, Color.softRed);
            fig.data.name = "test12312";
            var fig2 = G.shelfManager.GetFigureFromSlot(12);
            UIDebug.Log(fig2.data.name, Color.softRed);
            UIDebug.Log(fig.data.name, Color.softRed);

        }
        var enemy1 = new Figure("skeleton", "Скелет", 500, 15, 4, 5, 1);
        enemy1.skills.Add(new Skill("Удар", Skill.SkillType.Attack, 30, 1));
        G.battleSystem.StartBattle(new LevelData("test", 1, new List<Figure>(new Figure[] { enemy1 }), new BattleReward(0, 0, new List<string>())));

    }

    void Update()
    {
        string time = G.timer.GetTimeFormatted();
        G.ui.timer.text = time;

        if (Input.GetKeyUp(KeyCode.F))
        {
            G.shopFigures.BuyRandomFigure();
        }
    }
}
