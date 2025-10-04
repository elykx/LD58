using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

// Класс данных для фигурки
[Serializable]
public class Figure
{
    public string id;
    public string name;
    public string description;
    public int rarity;
    public Sprite sprite;
    public int cost;
    public int lvl;

    // Статы
    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int speed;
    public int defense;

    // Позиция в бою (0-3)
    public int battlePosition = -1;
    public bool isEnemy = false;

    // Скиллы
    public List<Skill> skills = new List<Skill>();
    private Dictionary<string, int> skillCooldowns = new Dictionary<string, int>();
    // Позиция в шкафу
    public Vector2Int shelfPosition;

    public Figure(string id, string name, int hp, int dmg, int spd, int def, int lvl)
    {
        this.id = id;
        this.name = name;
        maxHealth = hp;
        currentHealth = hp;
        damage = dmg;
        speed = spd;
        defense = def;
        this.lvl = lvl;
    }

    public bool IsAlive() => currentHealth > 0;

    public void TakeDamage(int amount)
    {
        int actualDamage = Mathf.Max(1, amount - defense / 2);
        currentHealth = Mathf.Max(0, currentHealth - actualDamage);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public bool CanUseSkill(Skill skill)
    {
        if (!skillCooldowns.ContainsKey(skill.skillName))
            return true;
        return skillCooldowns[skill.skillName] <= 0;
    }

    public void UseSkill(Skill skill)
    {
        skillCooldowns[skill.skillName] = skill.cooldown;
    }

    public void ReduceCooldowns()
    {
        var keys = skillCooldowns.Keys.ToList();
        foreach (var key in keys)
        {
            skillCooldowns[key] = Mathf.Max(0, skillCooldowns[key] - 1);
        }
    }
}

// Класс для скиллов
[Serializable]
public class Skill
{
    public string skillName;
    public string description;
    public int cooldown;
    public SkillType type;
    public Sprite sprite;

    // Параметры скилла
    public int power; // Урон или хил
    public int minRange; // Минимальная позиция цели (0-3)
    public int maxRange; // Максимальная позиция цели (0-3)
    public bool canTargetAllies; // Может ли использовать на союзников
    public bool canTargetEnemies; // Может ли использовать на врагов

    public enum SkillType
    {
        Attack,
        Heal,
        Buff,
        Debuff
    }

    public Skill(string name, SkillType type, int power, int cooldown, int minRange = 0, int maxRange = 3)
    {
        this.skillName = name;
        this.type = type;
        this.power = power;
        this.cooldown = cooldown;
        this.minRange = minRange;
        this.maxRange = maxRange;

        canTargetEnemies = type == SkillType.Attack || type == SkillType.Debuff;
        canTargetAllies = type == SkillType.Heal || type == SkillType.Buff;
    }


}