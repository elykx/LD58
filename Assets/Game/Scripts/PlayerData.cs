using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int money = 100; // $ в кармашке

    public int level = 1; // уровень игрока

    void Awake()
    {
        G.playerData = this;
    }
}