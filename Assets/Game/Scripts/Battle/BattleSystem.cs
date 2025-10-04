using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("События")]
    public UnityEvent<BattleReward> OnBattleVictory;
    public UnityEvent OnBattleDefeat;

    public List<BattleFigureView> playerViews = new();
    public List<BattleFigureView> enemyViews = new();
    public Queue<BattleFigureView> turnOrder = new();
    private BattleFigureView currentFighter;
    private Skill selectedSkill;

    public BattleState state;
    private LevelData currentLevel;
    private bool battleActive = false;


    void Awake()
    {
        G.battleSystem = this;
    }

    public void StartBattle(LevelData level)
    {
        if (battleActive)
        {
            UIDebug.Log("Битва уже идёт!");
            return;
        }

        currentLevel = level;
        battleActive = true;

        // Очищаем предыдущие данные
        ClearBattle();

        PlayerFigures();
        EnemyFigures(level);

        // Начинаем битву
        CalculateTurnOrder();
        NextTurn();

        UIDebug.Log($"Битва началась! Уровень: {level.levelName}");
    }

    public void EndBattle()
    {
        battleActive = false;
        ClearBattle();
    }

    // 0. Очистка битвы
    private void ClearBattle()
    {
        // Удаляем старые фигурки
        foreach (var view in playerViews)
        {
            if (view != null)
                Destroy(view.gameObject);
        }
        foreach (var view in enemyViews)
        {
            if (view != null)
                Destroy(view.gameObject);
        }

        playerViews.Clear();
        enemyViews.Clear();
        turnOrder.Clear();
        currentFighter = null;
        selectedSkill = null;
    }

    // 1. Собрать мои фигурки
    private void PlayerFigures()
    {
        List<string> playerFigures = new();
        var fig1 = G.shelfManager.GetFigureFromSlot(0);
        if (fig1 != null)
            playerFigures.Add(fig1.FigureId);
        var fig2 = G.shelfManager.GetFigureFromSlot(1);
        if (fig2 != null)
            playerFigures.Add(fig2.FigureId);
        var fig3 = G.shelfManager.GetFigureFromSlot(2);
        if (fig3 != null)
            playerFigures.Add(fig3.FigureId);
        var fig4 = G.shelfManager.GetFigureFromSlot(3);
        if (fig4 != null)
            playerFigures.Add(fig4.FigureId);

        if (playerFigures.Count == 0)
        {
            UIDebug.Log("Нет доступных бойцов!");
            battleActive = false;
            return;
        }

        SetupTeam(playerFigures.ToArray(), false, playerPositions, playerViews);

    }

    // 2. Собрать врагов из уровня
    private void EnemyFigures(LevelData level)
    {
        SetupTeam(level.enemies.ToArray(), true, enemyPositions, enemyViews);
    }

    private void SetupTeam(string[] figures, bool isEnemy, Transform[] positions, List<BattleFigureView> list)
    {
        list.Clear();
        for (int i = 0; i < figures.Length && i < positions.Length; i++)
        {
            G.figureManager.GetFigure(figures[i]).isEnemy = isEnemy;
            var view = Instantiate(battleFigurePrefab, positions[i]).GetComponent<BattleFigureView>();
            view.Initialize(figures[i]);
            list.Add(view);
        }
    }

    // 3. Рассчитываем очередь
    private void CalculateTurnOrder()
    {
        turnOrder.Clear();

        var allViews = new List<BattleFigureView>();
        allViews.AddRange(playerViews.Where(v => G.figureManager.GetFigure(v.FigureId).IsAlive()));
        allViews.AddRange(enemyViews.Where(v => G.figureManager.GetFigure(v.FigureId).IsAlive()));

        var sortedViews = allViews
            .OrderByDescending(v => G.figureManager.GetFigure(v.FigureId).speed + UnityEngine.Random.Range(-2, 3))
            .ToList();

        foreach (var view in sortedViews)
        {
            turnOrder.Enqueue(view);
        }

        battleUI.SetTurnOrder(turnOrder);
    }

    // 4. Следующий ход
    void NextTurn()
    {
        // Проверяем условия победы/поражения
        if (!playerViews.Any(v => G.figureManager.GetFigure(v.FigureId).IsAlive()))
        {
            state = BattleState.Defeat;
            battleUI.SetBattleState(state);
            UIDebug.Log("Поражение!");
            actionPanel.Hide();
            G.main.EndBattle();
            return;
        }
        if (!enemyViews.Any(v => G.figureManager.GetFigure(v.FigureId).IsAlive()))
        {
            state = BattleState.Victory;
            battleUI.SetBattleState(state);
            UIDebug.Log("Победа!");
            actionPanel.Hide();
            G.main.EndBattle();
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
        if (!G.figureManager.GetFigure(currentFighter.FigureId).IsAlive())
        {
            NextTurn();
            return;
        }

        // Подсвечиваем активного бойца
        ClearAllHighlights();
        currentFighter.SetActiveHighlight(true);

        if (G.figureManager.GetFigure(currentFighter.FigureId).isEnemy)
        {
            state = BattleState.EnemyTurn;
            battleUI.SetBattleState(state);
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.PlayerTurn;
            battleUI.SetBattleState(state);
            actionPanel.Set(G.figureManager.GetFigure(currentFighter.FigureId));
        }
    }

    public void OnSkillSelected(Skill skill)
    {
        selectedSkill = skill;
        state = BattleState.WaitingForTarget;
        battleUI.SetBattleState(state);

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
            targets = enemyViews.Where(v => G.figureManager.GetFigure(v.FigureId).IsAlive()).ToList();
        }
        else if (skill.type == Skill.SkillType.Heal)
        {
            // Лечение - только союзники
            targets = playerViews.Where(v => G.figureManager.GetFigure(v.FigureId).IsAlive()).ToList();
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
            UIDebug.Log("Неверная цель!");
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

            value = Mathf.Max(1, G.figureManager.GetFigure(actor.FigureId).damage / 2 + skill.power - G.figureManager.GetFigure(target.FigureId).defense / 2);
            G.figureManager.GetFigure(target.FigureId).TakeDamage(value);
            target.PlayHitAnimation();
            target.ShowDamageText(value, false);
        }
        else if (skill.type == Skill.SkillType.Heal)
        {
            value = skill.power;
            G.figureManager.GetFigure(target.FigureId).Heal(value);
            target.ShowDamageText(value, true);
        }


        yield return new WaitForSeconds(0.5f);

        // Проверяем смерть
        if (!G.figureManager.GetFigure(target.FigureId).IsAlive())
        {
            target.PlayDeathAnimation();
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

        var alivePlayers = playerViews.Where(v => G.figureManager.GetFigure(v.FigureId).IsAlive()).ToList();
        if (alivePlayers.Count == 0)
        {
            NextTurn();
            yield break;
        }

        var target = alivePlayers[UnityEngine.Random.Range(0, alivePlayers.Count)];

        // Подсвечиваем цель
        target.SetTargetHighlight(true);
        yield return new WaitForSeconds(0.5f);

        var enemySkill = G.figureManager.GetFigure(currentFighter.FigureId).skills.FirstOrDefault()
            ?? new Skill("Удар", Skill.SkillType.Attack, G.figureManager.GetFigure(currentFighter.FigureId).damage, 1);

        yield return ExecuteAction(currentFighter, target, enemySkill);
    }

}
