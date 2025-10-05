using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum BattleState
{
    Preparing,
    InProgress,
    Victory,
    Defeat,
    Paused
}

public class BattleFighterData
{
    public BattleFigureView view;
    public Figure figure;
    public float actionBar = 0f;
    public bool isActing = false;
}

[System.Serializable]
public class BattleReward
{
    public int gold;
    public int experience;
    public List<string> items;

    public BattleReward(int gold, int exp, List<string> items)
    {
        this.gold = gold;
        this.experience = exp;
        this.items = items;
    }
    public BattleReward() { }
}

public class BattleSystem : MonoBehaviour
{
    [Header("Позиции")]
    public Transform[] playerPositions;
    public Transform[] enemyPositions;
    public GameObject battleFigurePrefab;

    [Header("UI")]
    public BattleUI battleUI;

    [Header("События")]
    public UnityEvent<BattleReward> OnBattleVictory;
    public UnityEvent OnBattleDefeat;

    [Header("Настройки боя")]
    public float actionDelay = 0.5f;
    public float turnCalculationInterval = 0.1f;

    private List<BattleFighterData> allFighters = new();
    public LevelData currentLevel;
    private bool battleActive = false;
    private float battleTimer = 0f;
    public BattleState state;

    // public List<BattleFigureView> playerViews = new();
    // public List<BattleFigureView> enemyViews = new();
    // public Queue<BattleFigureView> turnOrder = new();
    // private BattleFigureView currentFighter;
    // private Skill selectedSkill;

    // public BattleState state;
    // private LevelData currentLevel;
    // private bool battleActive = false;


    void Awake()
    {
        G.battleSystem = this;
    }

    void Update()
    {
        if (!battleActive || state != BattleState.InProgress)
            return;

        battleTimer += Time.deltaTime;

        // Обновляем шкалы действий только для живых бойцов
        foreach (var fighter in allFighters.Where(f => f.figure.IsAlive()))
        {
            fighter.actionBar += fighter.figure.speed * Time.deltaTime;

            if (fighter.actionBar >= 100f && !fighter.isActing)
            {
                StartCoroutine(PerformAction(fighter));
            }
        }
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
        battleTimer = 0f;

        ClearBattle();
        SetupTeam(true); // Игроки
        SetupTeam(false); // Враги

        state = BattleState.InProgress;
        battleUI?.SetBattleState(state);

        UIDebug.Log($"Автобатл начался! Уровень: {level.levelId}");
    }

    public void EndBattle()
    {
        battleActive = false;
        ClearBattle();
    }

    public void PauseBattle()
    {
        if (state == BattleState.InProgress)
        {
            state = BattleState.Paused;
            battleUI?.SetBattleState(state);
        }
    }

    public void ResumeBattle()
    {
        if (state == BattleState.Paused)
        {
            state = BattleState.InProgress;
            battleUI?.SetBattleState(state);
        }
    }

    // 0. Очистка битвы
    private void ClearBattle()
    {
        foreach (var fighter in allFighters)
        {
            Destroy(fighter.view.gameObject);
        }

        allFighters.Clear();
    }

    // 1. Собрать фигурки
    private void SetupTeam(bool isPlayer)
    {
        Transform[] positions = isPlayer ? playerPositions : enemyPositions;
        string[] figures = isPlayer
            ? GetPlayerFigures()
            : currentLevel.enemyIds.ToArray();

        Debug.Log($"Setup team: {string.Join(", ", figures)}");
        for (int i = 0; i < figures.Length && i < positions.Length; i++)
        {
            var figure = G.figureManager.GetFigure(figures[i]);
            Debug.Log($"Setup figure: {figure.name} + {figure.sprite}");
            figure.isEnemy = !isPlayer;
            figure.battlePosition = i;

            var view = Instantiate(battleFigurePrefab, positions[i]).GetComponent<BattleFigureView>();
            view.Initialize(figures[i]);

            var fighterData = new BattleFighterData
            {
                view = view,
                figure = figure,
                actionBar = Random.Range(0f, 30f) // Случайный старт
            };

            allFighters.Add(fighterData);
        }
    }

    private string[] GetPlayerFigures()
    {
        List<string> playerFigures = new();

        for (int i = 0; i < 4; i++)
        {
            var fig = G.shelfManager.GetFigureFromSlot(i);
            if (fig != null)
                playerFigures.Add(fig.FigureId);
        }

        if (playerFigures.Count == 0)
        {
            UIDebug.Log("Нет доступных бойцов!");
            battleActive = false;
        }

        return playerFigures.ToArray();
    }

    private IEnumerator PerformAction(BattleFighterData fighter)
    {
        fighter.isActing = true;
        fighter.actionBar = 0f;

        // Выбор скилла и цели
        var skill = ChooseSkill(fighter.figure);
        var target = ChooseTarget(fighter, skill);

        if (target == null)
        {
            fighter.isActing = false;
            yield break;
        }

        // Подсветка актора и цели
        fighter.view.SetActiveHighlight(true);
        target.view.SetTargetHighlight(true);

        yield return new WaitForSeconds(actionDelay);

        // Выполнение действия
        yield return ExecuteSkill(fighter, target, skill);

        // Убираем подсветку
        fighter.view.SetActiveHighlight(false);
        target.view.SetTargetHighlight(false);

        // Уменьшаем кулдауны
        fighter.figure.ReduceCooldowns();

        fighter.isActing = false;

        // Проверяем условия победы/поражения
        CheckBattleEnd();
    }

    private Skill ChooseSkill(Figure figure)
    {
        var availableSkills = figure.skills
            .Where(s => figure.CanUseSkill(s))
            .ToList();

        if (availableSkills.Count == 0)
            return null;

        // Приоритеты действий
        var weightedSkills = availableSkills.Select(skill =>
        {
            int weight = CalculateSkillWeight(figure, skill);
            return (skill, weight);
        }).ToList();

        return weightedSkills.OrderByDescending(ws => ws.weight).First().skill;
    }

    private int CalculateSkillWeight(Figure figure, Skill skill)
    {
        int weight = 0;

        switch (skill.type)
        {
            case Skill.SkillType.Attack:
                weight += 10; // Приоритет атаки
                break;

            case Skill.SkillType.Heal when figure.currentHealth < figure.maxHealth * 0.5f:
                weight += 15; // Лечение при низком здоровье
                break;

            case Skill.SkillType.Buff when battleTimer < 5f:
                weight += 8; // Баффы в начале боя
                break;
        }

        return weight;
    }

    private BattleFighterData ChooseTarget(BattleFighterData actor, Skill skill)
    {
        var validTargets = GetValidTargets(actor, skill);

        if (validTargets.Count == 0)
            return null;

        // Оцениваем эффективность для каждой цели
        var scoredTargets = validTargets.Select(target =>
        {
            float score = CalculateTargetScore(actor, target, skill);
            return (target, score);
        }).ToList();

        return scoredTargets.OrderByDescending(t => t.score).First().target;
    }

    private List<BattleFighterData> GetValidTargets(BattleFighterData actor, Skill skill)
    {
        bool isPlayerTeam = !actor.figure.isEnemy;
        var targetPool = isPlayerTeam
            ? allFighters.Where(f => f.figure.isEnemy)
            : allFighters.Where(f => !f.figure.isEnemy);

        return targetPool
            .Where(f => f.figure.IsAlive() &&
                        f.figure.battlePosition >= skill.minRange &&
                        f.figure.battlePosition <= skill.maxRange)
            .ToList();
    }

    private float CalculateTargetScore(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        switch (skill.type)
        {
            case Skill.SkillType.Attack:
                return (actor.figure.damage + skill.power) / (target.figure.defense + 1) *
                       (target.figure.currentHealth / (float)target.figure.maxHealth);

            case Skill.SkillType.Heal:
                return (target.figure.maxHealth - target.figure.currentHealth) / (float)target.figure.maxHealth;

            case Skill.SkillType.Buff:
                return target.figure.damage / (float)target.figure.maxHealth;

            default:
                return 1f;
        }
    }

    private IEnumerator ExecuteSkill(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        if (skill.cooldown > 0)
            actor.figure.UseSkill(skill);

        UIDebug.Log($"{actor.figure.name} использует {skill.skillName} на {target.figure.name}");

        switch (skill.type)
        {
            case Skill.SkillType.Attack:
                yield return ExecuteAttack(actor, target, skill);
                break;

            case Skill.SkillType.Heal:
                yield return ExecuteHeal(actor, target, skill);
                break;

            case Skill.SkillType.Buff:
                yield return ExecuteBuff(actor, target, skill);
                break;

            case Skill.SkillType.Debuff:
                yield return ExecuteDebuff(actor, target, skill);
                break;
        }

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator ExecuteAttack(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        yield return actor.view.AttackAnimation(target.view.transform.position);

        int damage = Mathf.Max(1, actor.figure.damage / 2 + skill.power - target.figure.defense / 2);
        target.figure.TakeDamage(damage);

        target.view.PlayHitAnimation();
        target.view.ShowDamageText(damage, false);

        if (!target.figure.IsAlive())
        {
            target.view.PlayDeathAnimation();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator ExecuteHeal(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        int healAmount = skill.power;
        target.figure.Heal(healAmount);
        target.view.ShowDamageText(healAmount, true);

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator ExecuteBuff(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        target.figure.damage += skill.power;
        //TODO target.view.ShowBuffEffect(); 

        UIDebug.Log($"{target.figure.name} получает бафф! Урон +{skill.power}");

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator ExecuteDebuff(BattleFighterData actor, BattleFighterData target, Skill skill)
    {
        target.figure.defense = Mathf.Max(0, target.figure.defense - skill.power);
        //TODO target.view.ShowDebuffEffect();

        UIDebug.Log($"{target.figure.name} получает дебафф! Защита -{skill.power}");

        yield return new WaitForSeconds(0.3f);
    }

    private void CheckBattleEnd()
    {
        bool playersAlive = allFighters.Any(f => !f.figure.isEnemy && f.figure.IsAlive());
        bool enemiesAlive = allFighters.Any(f => f.figure.isEnemy && f.figure.IsAlive());

        if (!playersAlive)
        {
            state = BattleState.Defeat;
            battleUI?.SetBattleState(state);
            UIDebug.Log("Поражение!");
            OnBattleDefeat?.Invoke();
            StartCoroutine(DelayedEndBattle());
        }
        else if (!enemiesAlive)
        {
            state = BattleState.Victory;
            battleUI?.SetBattleState(state);
            UIDebug.Log("Победа!");

            var reward = new BattleReward
            {
                gold = currentLevel.reward.gold,
                experience = currentLevel.reward.experience
            };
            OnBattleVictory?.Invoke(reward);
            StartCoroutine(DelayedEndBattle());
        }
    }

    private IEnumerator DelayedEndBattle()
    {
        yield return new WaitForSeconds(2f);
        G.main.EndBattle();
    }

}
