using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FigureBattlePanel : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup skillPanel;
    [SerializeField] private GameObject skillButtonPrefab;

    public TextMeshProUGUI figureName;

    void Start()
    {
        Hide();
    }
    public void Set(Figure figure)
    {
        if (skillPanel != null)
        {
            foreach (Transform child in skillPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var skill in figure.skills)
            {
                var buttonObj = Instantiate(skillButtonPrefab, skillPanel.transform);
                var button = buttonObj.GetComponent<Button>();
                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = skill.skillName + "\n" + skill.power;
                }

                // Привязка клика к навыку
                var skillCopy = skill; // Важно для замыкания
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnSkillClicked(skillCopy));
            }
        }

        figureName.text = figure.name;
        Show();
    }

    private void OnSkillClicked(Skill skill)
    {
        Debug.Log("Skill clicked: " + skill.skillName);
        if (G.battleSystem != null)
        {
            G.battleSystem.OnSkillSelected(skill); // Передаёт навык в BattleSystem
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}