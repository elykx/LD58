using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BattleState
{
    PlayerTurn,
    WaitingForTarget,
    EnemyTurn,
    Victory,
    Defeat
}

public class BattleSystem : MonoBehaviour
{
    [Header("Позиции")]
    public Transform[] playerPositions;
    public Transform[] enemyPositions;
    public GameObject battleFigurePrefab;

    [Header("UI")]
    public FigureBattlePanel actionPanel;
    public BattleUI battleUI;

    private List<BattleFigureView> playerViews = new();
    private List<BattleFigureView> enemyViews = new();
    private Queue<BattleFigureView> turnOrder = new();
    private BattleFigureView currentFighter;
    private Skill selectedSkill;

    public BattleState state;

    void Awake()
    {
        G.battleSystem = this;
    }
    void Start()
    {
        SetupExampleBattle();
        StartBattle();
    }

    private void StartBattle()
    {
        CalculateTurnOrder();
        NextTurn();
    }


    private void CalculateTurnOrder()
    {
        turnOrder.Clear();

        var allViews = new List<BattleFigureView>();
        allViews.AddRange(playerViews.Where(v => v.figureData.IsAlive()));
        allViews.AddRange(enemyViews.Where(v => v.figureData.IsAlive()));

        var sortedViews = allViews
            .OrderByDescending(v => v.figureData.speed + UnityEngine.Random.Range(-2, 3))
            .ToList();

        foreach (var view in sortedViews)
        {
            turnOrder.Enqueue(view);
        }

        battleUI.SetTurnOrder(turnOrder);
    }

    void SetupExampleBattle()
    {
        var warrior = new Figure("warrior", "Воин", 100, 25, 5, 10);
        warrior.skills.Add(new Skill("Удар", Skill.SkillType.Attack, 40, 1));
        warrior.skills.Add(new Skill("Сильный удар", Skill.SkillType.Attack, 60, 2));

        var healer = new Figure("healer", "Целитель", 60, 10, 6, 3);
        healer.skills.Add(new Skill("Лечение", Skill.SkillType.Heal, 35, 2));
        healer.skills.Add(new Skill("Удар", Skill.SkillType.Attack, 20, 1));

        SetupTeam(new[] { warrior, healer }, false, playerPositions, playerViews);

        var enemy1 = new Figure("skeleton", "Скелет", 50, 15, 4, 5);
        enemy1.skills.Add(new Skill("Удар", Skill.SkillType.Attack, 30, 1));

        var enemy2 = new Figure("skeleton", "Скелет-лучник", 40, 20, 3, 8);
        enemy2.skills.Add(new Skill("Выстрел", Skill.SkillType.Attack, 35, 1));

        SetupTeam(new[] { enemy1, enemy2 }, true, enemyPositions, enemyViews);

        state = BattleState.PlayerTurn;
    }

    void SetupTeam(Figure[] figures, bool isEnemy, Transform[] positions, List<BattleFigureView> list)
    {
        list.Clear();
        for (int i = 0; i < figures.Length && i < positions.Length; i++)
        {
            figures[i].isEnemy = isEnemy;
            var view = Instantiate(battleFigurePrefab, positions[i]).GetComponent<BattleFigureView>();
            view.Initialize(figures[i]);
            list.Add(view);
        }
    }

    void NextTurn()
    {
        // Проверяем условия победы/поражения
        if (!playerViews.Any(v => v.figureData.IsAlive()))
        {
            state = BattleState.Defeat;
            battleUI.SetBattleState(state);
            Log("Поражение!");
            actionPanel.Hide();
            return;
        }
        if (!enemyViews.Any(v => v.figureData.IsAlive()))
        {
            state = BattleState.Victory;
            battleUI.SetBattleState(state);
            Log("Победа!");
            actionPanel.Hide();
            return;
        }

        // Если очередь пуста, пересчитываем порядок ходов
        if (turnOrder.Count == 0)
        {
            CalculateTurnOrder();
        }

        // Берём следующего бойца из очереди
        currentFighter = turnOrder.Dequeue();

        // Если боец мёртв, переходим к следующему
        if (!currentFighter.figureData.IsAlive())
        {
            NextTurn();
            return;
        }

        // Подсвечиваем активного бойца
        ClearAllHighlights();
        currentFighter.SetActiveHighlight(true);

        if (currentFighter.figureData.isEnemy)
        {
            state = BattleState.EnemyTurn;
            battleUI.SetBattleState(state);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            ShowActionPanel();
        }
    }

    void ShowActionPanel()
    {
        state = BattleState.PlayerTurn;
        battleUI.SetBattleState(state);
        actionPanel.Set(currentFighter.figureData);
        Log($"Ход: {currentFighter.figureData.name}");
    }

    public void OnSkillSelected(Skill skill)
    {
        Debug.Log("Skill selected in bs: " + skill.skillName);
        selectedSkill = skill;
        state = BattleState.WaitingForTarget;
        battleUI.SetBattleState(state);
        Log($"Выберите цель для {skill.skillName}");

        // Подсвечиваем возможные цели
        HighlightValidTargets(skill);
    }

    private void HighlightValidTargets(Skill skill)
    {
        List<BattleFigureView> validTargets = GetValidTargets(skill);

        foreach (var view in validTargets)
        {
            view.SetTargetHighlight(true);
        }
    }

    private List<BattleFigureView> GetValidTargets(Skill skill)
    {
        List<BattleFigureView> targets = new();

        if (skill.type == Skill.SkillType.Attack)
        {
            // Атака - только враги
            targets = enemyViews.Where(v => v.figureData.IsAlive()).ToList();
        }
        else if (skill.type == Skill.SkillType.Heal)
        {
            // Лечение - только союзники
            targets = playerViews.Where(v => v.figureData.IsAlive()).ToList();
        }

        return targets;
    }

    public void OnTargetSelected(BattleFigureView target)
    {
        if (state != BattleState.WaitingForTarget) return;

        // Проверяем, является ли цель валидной
        List<BattleFigureView> validTargets = GetValidTargets(selectedSkill);
        if (!validTargets.Contains(target))
        {
            Log("Неверная цель!");
            return;
        }

        ClearAllHighlights();
        actionPanel.Hide();
        StartCoroutine(ExecuteAction(currentFighter, target, selectedSkill));
    }

    private void ClearAllHighlights()
    {
        foreach (var view in playerViews)
        {
            view.SetActiveHighlight(false);
            view.SetTargetHighlight(false);
        }
        foreach (var view in enemyViews)
        {
            view.SetActiveHighlight(false);
            view.SetTargetHighlight(false);
        }
    }

    IEnumerator ExecuteAction(BattleFigureView actor, BattleFigureView target, Skill skill)
    {
        int value = 0;

        // Анимация атаки
        if (skill.type == Skill.SkillType.Attack)
        {
            yield return actor.AttackAnimation(target.transform.position);

            value = Mathf.Max(1, actor.figureData.damage / 2 + skill.power - target.figureData.defense / 2);
            target.figureData.TakeDamage(value);
            target.PlayHitAnimation();
            target.ShowDamageText(value, false);
        }
        else if (skill.type == Skill.SkillType.Heal)
        {
            value = skill.power;
            target.figureData.Heal(value);
            target.ShowDamageText(value, true);
        }

        target.UpdateHealthBar();
        Log($"{actor.figureData.name} использует {skill.skillName} на {target.figureData.name} ({value})");

        yield return new WaitForSeconds(0.5f);

        // Проверяем смерть
        if (!target.figureData.IsAlive())
        {
            target.PlayDeathAnimation();
            Log($"{target.figureData.name} повержен!");
            yield return new WaitForSeconds(0.5f);
        }

        // Сбрасываем выбор
        selectedSkill = null;

        // Следующий ход
        NextTurn();
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        var alivePlayers = playerViews.Where(v => v.figureData.IsAlive()).ToList();
        if (alivePlayers.Count == 0)
        {
            NextTurn();
            yield break;
        }

        var target = alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Count)];

        // Подсвечиваем цель
        target.SetTargetHighlight(true);
        yield return new WaitForSeconds(0.5f);

        var enemySkill = currentFighter.figureData.skills.FirstOrDefault()
            ?? new Skill("Удар", Skill.SkillType.Attack, currentFighter.figureData.damage, 1);

        yield return ExecuteAction(currentFighter, target, enemySkill);
    }

    void Log(string text)
    {
        UIDebug.Log(text);
    }
}
