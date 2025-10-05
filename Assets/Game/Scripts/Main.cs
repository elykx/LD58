using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK;
using Runtime;
using UnityEngine;

public class Main : MonoBehaviour
{
    private bool nightReady = false;
    public GameObject shop;

    private void Awake()
    {
        G.main = this;
        shop.SetActive(false);
    }

    private void Start()
    {
        // Init
        GameAnalytics.Initialize();
        CMS.Init();
        
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

    public void StartBattle()
    {
        if (nightReady)
        {
            G.cameraMove.MoveCameraToBed();
            G.battleSystem.StartBattle(LevelManager.GetLevelById("level_1"));
            G.playerData.current_pos = "battle";
        }
    }

    public void EndBattle()
    {
        nightReady = false;
        G.timer.StartDay();
        G.battleSystem.EndBattle();
        G.cameraMove.MoveCameraToBase();
        G.playerData.current_pos = "main";
        G.playerData.shopAlreadyOpened = false;
        Debug.Log("Бой завершён, день пошёл заново!");
    }

    public void GoShop()
    {
        if (G.playerData.current_pos != "main" || G.playerData.shopAlreadyOpened) return;
        G.cameraMove.MoveCameraToShop();
        shop.SetActive(true);
        G.shopFigures.GenerateShopFigures(G.playerData.level);
        G.playerData.current_pos = "shop";
        G.playerData.shopAlreadyOpened = true;
    }

    public void CloseShop()
    {
        shop.SetActive(false);
        G.cameraMove.MoveCameraToBase();
        G.playerData.current_pos = "main";
    }

    private void OnDestroy()
    {
        G.timer.OnNightStart -= OnNightStart;
    }
}
