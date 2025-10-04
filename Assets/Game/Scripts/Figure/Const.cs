using System.Collections.Generic;
using UnityEngine;
using static Skill;

public static class FigureConsts
{
    public static Figure baseFigure;

    public static void Init()
    {
        Skill skillAttack = new Skill("Атака", Skill.SkillType.Attack, 10, 1)
        {
            description = "Наносит базовый удар.",
            sprite = Resources.Load<Sprite>("Images/ability_0"),

        };

        baseFigure = new Figure
        {
            id = "baseFigure",
            name = "Базовая фигура",
            description = "Простая фигура для теста боёвки.",
            rarity = 1,
            cost = 10,
            lvl = 1,

            maxHealth = 100,
            currentHealth = 100,
            damage = 10,
            speed = 5,
            defense = 2,

            sprite = Resources.Load<Sprite>("Images/figure1"),
            skills = new List<Skill> { skillAttack }
        };
    }
}