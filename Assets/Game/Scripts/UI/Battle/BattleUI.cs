using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI state;
    public HorizontalLayoutGroup turnOrderList;
    public GameObject figureIconPrefab;

    private void Awake()
    {
        state.gameObject.SetActive(false);
        turnOrderList.gameObject.SetActive(false);
    }

    public void SetBattleState(BattleState _state)
    {
        state.gameObject.SetActive(true);
        switch (_state)
        {
            case BattleState.PlayerTurn:
                state.text = "Player Turn";
                break;
            case BattleState.WaitingForTarget:
                state.text = "Waiting For Target";
                break;
            case BattleState.EnemyTurn:
                state.text = "Enemy Turn";
                break;
            case BattleState.Victory:
                state.text = "Victory";
                break;
            case BattleState.Defeat:
                state.text = "Defeat";
                break;
        }
    }

    public void SetTurnOrder(Queue<BattleFigureView> turnOrder)
    {
        turnOrderList.gameObject.SetActive(true);
        foreach (Transform child in turnOrderList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var figure in turnOrder)
        {
            var icon = Instantiate(figureIconPrefab, turnOrderList.transform);
            var figIcon = icon.GetComponent<FigureIcon>();
            figIcon.SetIcon(figure.figureData.sprite);
            if (figure.figureData.isEnemy)
            {
                figIcon.SetBackground(Color.red);

            }
            var triggerTooltip = icon.GetComponent<TooltipTrigger>();
            if (triggerTooltip != null)
            {
                triggerTooltip.tooltipTitle = figure.figureData.name;
                triggerTooltip.tooltipContent = figure.figureData.currentHealth + "/" + figure.figureData.maxHealth;
            }

        }
    }
}