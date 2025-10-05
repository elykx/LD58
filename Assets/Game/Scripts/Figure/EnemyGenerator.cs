using System.Collections.Generic;
using System.Linq;

public static class EnemyGenerator
{
    public static List<Figure> GenerateEnemiesForLevel(LevelData levelData)
    {
        List<Figure> enemies = new List<Figure>();

        foreach (var enemyId in levelData.enemyIds)
        {
            var template = FigureGenerator.FigureTemplates.FirstOrDefault(t => t.baseId == enemyId);
            if (template != null)
            {
                var enemy = FigureGenerator.GenerateFigure(template, levelData.lvl);
                enemy.isEnemy = true;
                enemies.Add(enemy);
            }
        }

        return enemies;
    }
}