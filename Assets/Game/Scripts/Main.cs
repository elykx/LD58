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
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 1);
        G.shelfManager.AddFigureToSlot(G.shelfManager.baseFigure, 3);
    }

    void Update()
    {
        string time = G.timer.GetTimeFormatted();
        G.ui.timer.text = time;

        // Debug // TODO: remove
        if (Input.GetKeyUp(KeyCode.F))
        {
            G.shopFigures.BuyRandomFigure();
        }
    }

    private void OnNightStart()
    {
        nightReady = true;
    }

    public void OnBattleButton()
    {
        if (nightReady)
        {
            var enemy1 = new Figure("skeleton", "Скелет", 500, 15, 4, 5, 1);
            enemy1.skills.Add(new Skill("Удар", Skill.SkillType.Attack, 30, 1));
            G.battleSystem.StartBattle(new LevelData("test", 1, new List<Figure>(new Figure[] { enemy1 }), new BattleReward(0, 0, new List<string>())));
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
