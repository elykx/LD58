using UnityEngine;

public class Main : MonoBehaviour
{
    private void Awake()
    {
        G.main = this;
    }

    private void Start()
    {
        UIDebug.Log("Игра запущена.", Color.aquamarine);
    }
}
