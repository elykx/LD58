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
}