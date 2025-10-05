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
    public FigureClass figureClass; // Новое: класс фигурки

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

public enum FigureClass
{
    Melee,      // Ближний бой
    Ranged,     // Дальний бой (лучник)
    Mage,       // Маг
    Support,     // Поддержка (хилер)
    Unique
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
    description = "A sturdy close-combat warrior",
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
    figureClass = FigureClass.Melee
},
new FigureTemplate
{
    baseId = "archer",
    name = "Archer",
    description = "A fast long-range shooter",
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
    figureClass = FigureClass.Ranged
},
new FigureTemplate
{
    baseId = "mage",
    name = "Mage",
    description = "A powerful spellcaster with magic attacks",
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
    figureClass = FigureClass.Mage
},
new FigureTemplate
{
    baseId = "priest",
    name = "Priest",
    description = "A healer who supports the team",
    spritePaths = new List<string> { "Images/mage1", "Images/mage2" },
    baseStats = new StatRange
    {
        health = new MinMax(18, 24),
        damage = new MinMax(3, 5),
        speed = new MinMax(3, 4),
        defense = new MinMax(2, 4)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(3, 5),
        damage = new MinMax(1, 1),
        speed = new MinMax(0, 1),
        defense = new MinMax(1, 2)
    },
    baseCost = 14,
    costPerLevel = 4,
    rarity = 2,
    enemy = false,
    figureClass = FigureClass.Support
},
new FigureTemplate
{
    baseId = "unique",
    name = "Unique",
    description = "A versatile warrior",
    spritePaths = new List<string> { "Images/unique" },
    baseStats = new StatRange
    {
        health = new MinMax(50, 100),
        damage = new MinMax(12, 20),
        speed = new MinMax(3, 6),
        defense = new MinMax(4, 6)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(6, 7),
        damage = new MinMax(3, 6),
        speed = new MinMax(1, 2),
        defense = new MinMax(1, 2)
    },
    baseCost = 75,
    costPerLevel = 6,
    rarity = 6,
    enemy = false,
    figureClass = FigureClass.Unique
},
new FigureTemplate
{
    baseId = "rat",
    name = "Rat",
    description = "An angry rat",
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
    enemy = true,
    figureClass = FigureClass.Melee
},
new FigureTemplate
{
    baseId = "rabbit",
    name = "Rabbit",
    description = "A fast and sneaky rabbit",
    spritePaths = new List<string> { "Images/enemies" },
    baseStats = new StatRange
    {
        health = new MinMax(20, 30),
        damage = new MinMax(5, 8),
        speed = new MinMax(6, 8),
        defense = new MinMax(1, 2)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(2, 4),
        damage = new MinMax(1, 2),
        speed = new MinMax(1, 1),
        defense = new MinMax(0, 1)
    },
    baseCost = 15,
    costPerLevel = 3,
    rarity = 1,
    enemy = true,
    figureClass = FigureClass.Ranged
},
new FigureTemplate
{
    baseId = "dog",
    name = "Dog",
    description = "An aggressive guard dog",
    spritePaths = new List<string> { "Images/enemies" },
    baseStats = new StatRange
    {
        health = new MinMax(60, 80),
        damage = new MinMax(10, 15),
        speed = new MinMax(4, 6),
        defense = new MinMax(3, 5)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(6, 10),
        damage = new MinMax(2, 3),
        speed = new MinMax(0, 1),
        defense = new MinMax(1, 2)
    },
    baseCost = 30,
    costPerLevel = 3,
    rarity = 2,
    enemy = true,
    figureClass = FigureClass.Melee
},
new FigureTemplate
{
    baseId = "demon",
    name = "Demon",
    description = "A powerful demonic entity",
    spritePaths = new List<string> { "Images/enemies" },
    baseStats = new StatRange
    {
        health = new MinMax(100, 120),
        damage = new MinMax(20, 25),
        speed = new MinMax(3, 5),
        defense = new MinMax(5, 8)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(10, 15),
        damage = new MinMax(3, 5),
        speed = new MinMax(0, 1),
        defense = new MinMax(1, 3)
    },
    baseCost = 50,
    costPerLevel = 10,
    rarity = 4,
    enemy = true,
    figureClass = FigureClass.Mage
},
new FigureTemplate
{
    baseId = "ghost",
    name = "Ghost",
    description = "Ethereal and elusive",
    spritePaths = new List<string> { "Images/enemies" },
    baseStats = new StatRange
    {
        health = new MinMax(40, 60),
        damage = new MinMax(12, 18),
        speed = new MinMax(5, 7),
        defense = new MinMax(1, 3)
    },
    statGrowth = new StatRange
    {
        health = new MinMax(4, 6),
        damage = new MinMax(2, 4),
        speed = new MinMax(1, 1),
        defense = new MinMax(0, 1)
    },
    baseCost = 35,
    costPerLevel = 6,
    rarity = 3,
    enemy = true,
    figureClass = FigureClass.Ranged
},
    };

    // Генерация списка фигурок для уровня
    public static List<Figure> GetFiguresForLevel(int level)
    {
        List<Figure> figures = new List<Figure>();

        // 1. Находим доступные шаблоны
        var available = FigureTemplates.FindAll(t =>
        {
            if (t.enemy) return false;
            return t.baseCost <= G.playerData.money + t.costPerLevel * (level - 1) + t.rarity * 10;
        });

        if (available.Count == 0) return figures;

        // 2. Перемешиваем список (чтобы порядок был случайным)
        for (int i = 0; i < available.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, available.Count);
            var tmp = available[i];
            available[i] = available[rnd];
            available[rnd] = tmp;
        }

        // 3. Берём случайное количество типов (например 2–3)
        int typesToPick = UnityEngine.Random.Range(3, Mathf.Min(available.Count, 4));

        for (int i = 0; i < typesToPick; i++)
        {
            var template = available[i];


            figures.Add(GenerateFigure(template, level));

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
            cost = template.baseCost + template.costPerLevel * (level - 1),
            figureClass = template.figureClass
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

        AddSkillsByClass(figure, template.figureClass, level);

        return figure;
    }

    private static void AddSkillsByClass(Figure figure, FigureClass figureClass, int level)
    {
        switch (figureClass)
        {
            case FigureClass.Melee:
                figure.skills.Add(new Skill("Удар", Skill.SkillType.Attack, figure.damage, 0)
                {
                    description = "Ближняя атака",
                    minRange = 0,
                    maxRange = 1,
                    cooldown = 0,
                    sprite = Resources.Load<Sprite>("Images/ability_0")
                });
                break;

            case FigureClass.Ranged:
                figure.skills.Add(new Skill("Выстрел", Skill.SkillType.Attack, figure.damage, 0)
                {
                    description = "Дальняя атака",
                    minRange = 1,
                    maxRange = 4,
                    cooldown = 0,
                    sprite = Resources.Load<Sprite>("Images/ability_0")
                });
                break;

            case FigureClass.Mage:
                figure.skills.Add(new Skill("Снаряд", Skill.SkillType.Attack, figure.damage, 0)
                {
                    description = "Магическая дальняя атака",
                    minRange = 1,
                    maxRange = 4,
                    cooldown = 0,
                    sprite = Resources.Load<Sprite>("Images/ability_0")
                });
                break;

            case FigureClass.Support:
                figure.skills.Add(new Skill("Хил", Skill.SkillType.Heal, 10 + level * 2, 2)
                {
                    description = "Исцеляет союзника",
                    minRange = 0,
                    maxRange = 4,
                    cooldown = 2,
                    sprite = Resources.Load<Sprite>("Images/ability_1")
                });

                figure.skills.Add(new Skill("Благословение", Skill.SkillType.Buff, 3, 3)
                {
                    description = "Увеличивает защиту союзника",
                    minRange = 0,
                    maxRange = 4,
                    cooldown = 3,
                    sprite = Resources.Load<Sprite>("Images/ability_2")
                });
                break;
            case FigureClass.Unique:
                figure.skills.Add(new Skill("Удар", Skill.SkillType.Attack, figure.damage, 0)
                {
                    description = "Ближняя атака",
                    minRange = 0,
                    maxRange = 1,
                    cooldown = 0,
                    sprite = Resources.Load<Sprite>("Images/ability_0")
                });
                figure.skills.Add(new Skill("Выстрел", Skill.SkillType.Attack, figure.damage, 0)
                {
                    description = "Дальняя атака",
                    minRange = 1,
                    maxRange = 4,
                    cooldown = 0,
                    sprite = Resources.Load<Sprite>("Images/ability_0")
                });
                break;
        }
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