using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public TextMeshProUGUI timer;
    
    private void Awake()
    {
        G.ui = this;
    }

}

