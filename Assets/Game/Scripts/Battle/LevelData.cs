using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public string levelName;
    public int difficulty;
    public List<Figure> enemies = new();
    public BattleReward reward;

    public LevelData(string levelName, int difficulty, List<Figure> enemies, BattleReward reward)
    {
        this.levelName = levelName;
        this.difficulty = difficulty;
        this.enemies = enemies;
        this.reward = reward;
    }

    public BattleReward GetReward()
    {
        return reward;
    }
}