using UnityEngine;
using System.Collections.Generic;
using System;

// Класс данных для фигурки
[Serializable]
public class Figure
{
    public string id;
    public string name;
    public Sprite sprite;

    // Статы
    public int health;
    public int damage;
    public int speed;
    public int defense;

    // Скиллы
    public List<Skill> skills = new List<Skill>();

    // Позиция в шкафу
    public Vector2Int shelfPosition;

    public Figure(string id, string name, int hp, int dmg, int spd, int def)
    {
        this.id = id;
        this.name = name;
        health = hp;
        damage = dmg;
        speed = spd;
        defense = def;
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

    public enum SkillType
    {
        Attack,
        Heal,
        Buff,
        Debuff
    }
}