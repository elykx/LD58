using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform container;   
    [SerializeField] private GameObject messagePrefab;

    private static UIDebug instance;

    private class DebugMessage
    {
        public TextMeshProUGUI TextUI;
        public float ExpireTime;
    }

    private readonly List<DebugMessage> messages = new List<DebugMessage>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        float now = Time.time;
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            if (now > messages[i].ExpireTime)
            {
                Destroy(messages[i].TextUI.gameObject);
                messages.RemoveAt(i);
            }
        }
    }


    public static void Log(string text, float lifetime = 5f)
    {
        Log(text, Color.white, lifetime);
    }

    public static void Log(string text, Color color, float lifetime = 5f)
    {
        if (instance == null)
        {
            Debug.LogWarning("UIDebug не найден в сцене!");
            return;
        }

        var go = Instantiate(instance.messagePrefab, instance.container);
        var tmp = go.GetComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.color = color;

        instance.messages.Add(new DebugMessage
        {
            TextUI = tmp,
            ExpireTime = Time.time + lifetime
        });
    }

    public static void Clear()
    {
        if (instance == null) return;

        foreach (var msg in instance.messages)
            if (msg.TextUI != null)
                Destroy(msg.TextUI.gameObject);

        instance.messages.Clear();
    }
}
