using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class FigureTemplate
{
    public string baseId;
    public string name;
    public string description;
    public List<string> spritePaths;
    public StatRange baseStats;
    public StatRange statGrowth;
    public int baseCost;
    public int costPerLevel;
    public int rarity;
    public bool enemy;
}

// Диапазон статов
[Serializable]
public class StatRange
{
    public MinMax health;
    public MinMax damage;
    public MinMax speed;
    public MinMax defense;
}

[Serializable]
public class MinMax
{
    public int min;
    public int max;

    public MinMax(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

public static class FigureGenerator
{
    private static int uniqueIdCounter = 0;

    // Шаблоны фигурок с путями к спрайтам
    public static List<FigureTemplate> FigureTemplates = new List<FigureTemplate>()
    {
        new FigureTemplate
        {
            baseId = "knight",
            name = "Knight",
            description = "Крепкий воин ближнего боя",
            spritePaths = new List<string> { "Images/knight1", "Images/knight2", "Images/knight3" },
            baseStats = new StatRange
            {
                health = new MinMax(25, 35),
                damage = new MinMax(5, 8),
                speed = new MinMax(2, 4),
                defense = new MinMax(4, 6)
            },
            statGrowth = new StatRange
            {
                health = new MinMax(4, 6),
                damage = new MinMax(1, 2),
                speed = new MinMax(0, 1),
                defense = new MinMax(1, 2)
            },
            baseCost = 10,
            costPerLevel = 3,
            rarity = 1,
            enemy = false,
        },
        new FigureTemplate
        {
            baseId = "archer",
            name = "Archer",
            description = "Быстрый стрелок дальнего боя",
            spritePaths = new List<string> { "Images/archer1", "Images/archer2" },
            baseStats = new StatRange
            {
                health = new MinMax(15, 25),
                damage = new MinMax(7, 10),
                speed = new MinMax(4, 6),
                defense = new MinMax(1, 3)
            },
            statGrowth = new StatRange
            {
                health = new MinMax(3, 5),
                damage = new MinMax(2, 3),
                speed = new MinMax(1, 2),
                defense = new MinMax(0, 1)
            },
            baseCost = 12,
            costPerLevel = 4,
            rarity = 1,
            enemy = false,
        },
        new FigureTemplate
        {
            baseId = "mage",
            name = "Mage",
            description = "Могущественный маг с магическими атаками",
            spritePaths = new List<string> { "Images/mage1", "Images/mage2", "Images/mage3" },
            baseStats = new StatRange
            {
                health = new MinMax(12, 18),
                damage = new MinMax(9, 12),
                speed = new MinMax(3, 5),
                defense = new MinMax(0, 2)
            },
            statGrowth = new StatRange
            {
                health = new MinMax(2, 4),
                damage = new MinMax(2, 4),
                speed = new MinMax(1, 1),
                defense = new MinMax(0, 1)
            },
            baseCost = 15,
            costPerLevel = 5,
            rarity = 2,
            enemy = false,
        },
        new FigureTemplate
    {
        baseId = "rat",
        name = "Rat",
        description = "Крыса, злая",
        spritePaths = new List<string> { "Images/enemies" },
        baseStats = new StatRange
        {
            health = new MinMax(50, 70),
            damage = new MinMax(15, 20),
            speed = new MinMax(3, 5),
            defense = new MinMax(2, 4)
        },
        statGrowth = new StatRange
        {
            health = new MinMax(5, 8),
            damage = new MinMax(3, 5),
            speed = new MinMax(1, 2),
            defense = new MinMax(1, 2)
        },
        baseCost = 25,
        costPerLevel = 6,
        rarity = 3,
        enemy = true
    }
    };

    // Генерация списка фигурок для уровня
    public static List<Figure> GetFiguresForLevel(int level)
    {
        List<Figure> figures = new List<Figure>();

        foreach (var template in FigureTemplates)
        {
            if (!template.enemy)
            {
                // Проверяем доступность по стоимости
                int figureCost = template.baseCost + template.costPerLevel * (level - 1);
                if (figureCost <= G.playerData.money)
                {
                    // Генерируем несколько вариантов одного типа
                    int variantsCount = UnityEngine.Random.Range(1, 3);
                    for (int i = 0; i < variantsCount; i++)
                    {
                        figures.Add(GenerateFigure(template, level));
                    }
                }
            }
        }

        return figures;
    }

    public static Figure GenerateFigure(FigureTemplate template, int level)
    {
        string id;
        if (!template.enemy)
        {
            id = $"{template.baseId}_{uniqueIdCounter++}";
        }
        else
        {
            id = $"{template.baseId}";
        }
        Figure figure = new Figure
        {
            id = id,
            name = template.name,
            description = template.description,
            rarity = template.rarity,
            lvl = level,
            cost = template.baseCost + template.costPerLevel * (level - 1)
        };

        // Генерация статов
        figure.maxHealth = CalculateStat(template.baseStats.health, template.statGrowth.health, level);
        figure.currentHealth = figure.maxHealth;
        figure.damage = CalculateStat(template.baseStats.damage, template.statGrowth.damage, level);
        figure.speed = CalculateStat(template.baseStats.speed, template.statGrowth.speed, level);
        figure.defense = CalculateStat(template.baseStats.defense, template.statGrowth.defense, level);

        if (template.spritePaths.Count > 0)
        {
            string randomSpritePath = template.spritePaths[UnityEngine.Random.Range(0, template.spritePaths.Count)];
            string spriteName;
            Sprite[] allSprites;
            if (!template.enemy)
            {
                spriteName = randomSpritePath.Split('/').Last();
                allSprites = Resources.LoadAll<Sprite>("Images/characters");
            }
            else
            {
                spriteName = figure.id;
                allSprites = Resources.LoadAll<Sprite>("Images/enemies");
            }

            if (allSprites.Length > 0)
            {
                Sprite loadedSprite = allSprites.FirstOrDefault(s => s.name == spriteName);

                if (loadedSprite != null)
                {
                    figure.sprite = loadedSprite;
                }
            }
        }

        // Добавление базового скилла атаки
        Skill attackSkill = new Skill($"Атака {template.name}", Skill.SkillType.Attack, figure.damage, 1)
        {
            description = "Базовая атака",
            sprite = Resources.Load<Sprite>("Images/ability_0")
        };
        figure.skills.Add(attackSkill);

        return figure;
    }

    // Расчет стата с учетом уровня и рандомизации
    private static int CalculateStat(MinMax baseStat, MinMax growth, int level)
    {
        int baseValue = UnityEngine.Random.Range(baseStat.min, baseStat.max + 1);
        int growthValue = 0;

        for (int i = 1; i < level; i++)
        {
            growthValue += UnityEngine.Random.Range(growth.min, growth.max + 1);
        }

        return baseValue + growthValue;
    }
}