using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    [Header("Настройки")]
    public float dayDurationInSeconds = 180f; // 3 минуты = 180 секунд
    public float dayStartHour = 8f;
    public float dayEndHour = 24f;

    private float timer = 0f;
    private bool isRunning = false;

    public event Action OnNightStart;

    void Awake()
    {
        G.timer = this;
    }
    private void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / dayDurationInSeconds);

        float currentHour = Mathf.Lerp(dayStartHour, dayEndHour, progress);
        int hours = Mathf.FloorToInt(currentHour) % 24;
        int minutes = Mathf.FloorToInt((currentHour - Mathf.Floor(currentHour)) * 60f);

        if (progress >= 1f)
        {
            StopTimer();
            OnNightStart?.Invoke();
        }
    }

    public void StartDay()
    {
        timer = 0f;
        isRunning = true;
        Debug.Log("День начался в 08:00");
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetTimeFormatted()
    {
        float progress = Mathf.Clamp01(timer / dayDurationInSeconds);
        float currentHour = Mathf.Lerp(dayStartHour, dayEndHour, progress);
        int hours = Mathf.FloorToInt(currentHour) % 24;
        int minutes = Mathf.FloorToInt((currentHour - Mathf.Floor(currentHour)) * 60f);
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
}
