using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class LevelData
{
    public string levelId;
    public int lvl;
    public string description;
    public List<string> enemyIds; // Список ID врагов (например, "skeleton", "knight")
    public BattleReward reward;

    public LevelData(string levelId, int lvl, string description, List<string> enemyIds, BattleReward reward)
    {
        this.levelId = levelId;
        this.description = description;
        this.enemyIds = enemyIds;
        this.reward = reward;
    }
}


public static class LevelManager
{
    public static List<LevelData> Levels = new List<LevelData>()
    {
        new LevelData(
            "level_1",
            1,
            "Первый уровень - крыса",
            new List<string> { "rat" },
            new BattleReward(50, 10, new List<string> { "potion_health_small" })
        ),
        new LevelData(
            "level_2",
            2,
            "Второй уровень - немного сложнее",
            new List<string> { "rabbit", "demon" },
            new BattleReward(100, 20, new List<string> { "potion_mana_small" })
        ),
        new LevelData(
            "level_3",
            3,
            "Третий уровень - маги появляются",
            new List<string> { "rat", "demon", "ghost" },
            new BattleReward(150, 30, new List<string> { "sword_wooden" })
        ),
        new LevelData(
            "random",
            4,
            "Четвертый уровень - армия скелетов",
            new List<string> { "rat", "dog", "ghost", "demon" },
            new BattleReward(200, 40, new List<string> { "armor_leather" })
        ),
    };

    // Метод для получения уровня по его ID
    public static LevelData GetLevelById(string levelId)
    {
        var level = Levels.FirstOrDefault(level => level.levelId == levelId);
        var enemies = EnemyGenerator.GenerateEnemiesForLevel(level);
        foreach (var enemy in enemies)
        {
            G.figureManager.AddFigure(enemy);
        }
        return level;
    }
}