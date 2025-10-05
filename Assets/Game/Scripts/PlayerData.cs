using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int money = 100; // $ в кармашке

    public int level = 1; // уровень игрока/day

    public string level_id = "level_1";

    public string current_pos = "main";

    public bool shopAlreadyOpened = false;

    void Awake()
    {
        G.playerData = this;
    }

    public string GetLevel()
    {
        if (level == 1)
        {
            level_id = "level_1";
        }
        else if (level == 2)
        {
            level_id = "level_2";
        }
        else if (level == 3)
        {
            level_id = "level_3";
        }
        else
        {
            level_id = "random_level";
        }

        return level_id;
    }
}