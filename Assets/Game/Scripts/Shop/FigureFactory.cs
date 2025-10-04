using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FigureFactory
{
    public static List<Figure> AllFigures = new List<Figure>()
    {
        new Figure("knight", "Knight", 30, 6, 3, 5, 1)
        {
            cost = 10,
            // sprite = Resources.Load<Sprite>("Sprites/knight"),
            // skills = new List<Skill> { new Skill("Slash", 1, 3) }
        },
        new Figure("archer", "Archer", 20, 8, 5, 2, 1)
        {
            cost = 12,
            // sprite = Resources.Load<Sprite>("Sprites/archer")
        },
        new Figure("mage", "Mage", 15, 10, 4, 1, 1)
        {
            cost = 15,
        }
    };

    public static List<Figure> GetFiguresForLevel(int level)
    {
        // пример фильтрации: доступные до level*10 по цене
        return AllFigures.Where(f => f.cost <= level * 10).ToList();
    }

    public static Figure GetRandomFigure(int level)
    {
        var pool = GetFiguresForLevel(level);
        Debug.Log($"Получили фигуру из пула: {pool.Count}");
        if (pool.Count == 0)
        {
            return new Figure("archer", "Archer", 20, 8, 5, 2, 1)
            {
                cost = 12,
                // sprite = Resources.Load<Sprite>("Sprites/archer")
            };
        }

        var baseFigure = pool[UnityEngine.Random.Range(0, pool.Count)];

        // создаём копию, чтобы у каждой покупки были свои HP и скиллы
        return Clone(baseFigure);
    }

    public static Figure Clone(Figure original)
    {
        return new Figure(original.id, original.name, original.maxHealth, original.damage, original.speed, original.defense, original.lvl)
        {
            cost = original.cost,
            sprite = original.sprite,
            skills = new List<Skill>(original.skills)
        };
    }
}
