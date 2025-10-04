using System;
using UnityEngine;

[Serializable]
public class ShopSlot
{
    public Transform spawnPoint;
    [HideInInspector] public ViewShopFigure currentFigure;
}
