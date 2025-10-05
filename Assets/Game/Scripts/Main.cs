using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK;
using Runtime;
using UnityEngine;

public class Main : MonoBehaviour
{
    private bool nightReady = false;

    private void Awake()
    {
        G.main = this;
    }

    private void Start()
    {
        // Init
        GameAnalytics.Initialize();
        CMS.Init();
        UIDebug.Log("Игра запущена.", Color.aquamarine);


        // Start Day Loop
        G.timer.OnNightStart += OnNightStart;
        G.timer.StartDay();

        // Debug start figures //TODO: add real figures
        InitDefaultPlayerFiguresInventory();
    }

    private void InitDefaultPlayerFiguresInventory()
    {
        var base1 = FigureGenerator.GetFiguresForLevel(1)[0];
        var base2 = FigureGenerator.GetFiguresForLevel(1)[0];

        G.figureManager.AddFigure(base1);
        G.figureManager.AddFigure(base2);

        G.shelfManager.AddFigureToSlot(base1.id, 0);
        G.shelfManager.AddFigureToSlot(base2.id, 2);
    }

    void Update()
    {
        string time = G.timer.GetTimeFormatted();
        G.ui.timer.text = time;
        G.ui.money.text = StringUtils.FormatNumber(G.playerData.money);
    }

    private void OnNightStart()
    {
        nightReady = true;
    }

    public void OnBattleButton()
    {
        if (nightReady)
        {
            G.battleSystem.StartBattle(LevelManager.GetLevelById("level_1"));
        }
        else
        {
            Debug.Log("Бой нельзя запустить — день ещё идёт!");
        }
    }

    public void EndBattle()
    {
        nightReady = false;
        G.timer.StartDay();
        G.battleSystem.EndBattle();
        Debug.Log("Бой завершён, день пошёл заново!");
    }

    private void OnDestroy()
    {
        G.timer.OnNightStart -= OnNightStart;
    }
}
