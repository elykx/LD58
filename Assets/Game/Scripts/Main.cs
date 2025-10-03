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
    }
}
