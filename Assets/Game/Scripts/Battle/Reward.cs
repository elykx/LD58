using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleReward
{
    public int gold;
    public int experience;
    public List<string> items = new();

    public BattleReward(int gold, int experience, List<string> items) { this.gold = gold; this.experience = experience; this.items = items; }
}