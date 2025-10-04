using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI money;

    private void Awake()
    {
        G.ui = this;
    }

}

