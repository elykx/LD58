using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    [Header("Настройки времени")]
    public float dayTimeSpeed = 5.33f;   // День (16 часов) за 3 реальные минуты
    public float nightTimeSpeed = 4f;     // Ночь (8 часов) за 3 реальные минуты

    [Header("Периоды времени")]
    public float dayStartHour = 8f;   // День начинается в 8:00
    public float nightStartHour = 24f; // Ночь начинается в 24:00 (00:00)

    [Header("Игровые периоды")]
    public float dayLengthInHours = 16f;   // День длится 16 игровых часов (8:00-24:00)
    public float nightLengthInHours = 8f;  // Ночь длится 8 игровых часов (0:00-8:00)

    // Текущее игровое время
    private float currentHour = 8f;
    private float currentMinute = 0f;

    // Состояние
    private bool isDay = true;
    private bool isPaused = false;

    // События (можно подписаться из других скриптов)
    public event Action OnDayStart;
    public event Action OnNightStart;
    public event Action<int, int> OnTimeChanged; // (часы, минуты)
    void Awake()
    {
        G.timer = this;
    }

    void Start()
    {
        UIDebug.Log("Таймер запущен.", Color.aquamarine);
        currentHour = dayStartHour;
        currentMinute = 0f;
        isDay = true;
    }

    void Update()
    {
        if (isPaused) return;

        // Используем разную скорость для дня и ночи
        float currentSpeed = isDay ? dayTimeSpeed : nightTimeSpeed;

        // Увеличиваем время
        float minutesToAdd = (Time.deltaTime * currentSpeed);
        currentMinute += minutesToAdd;

        // Переводим минуты в часы
        if (currentMinute >= 60f)
        {
            currentHour += Mathf.Floor(currentMinute / 60f);
            currentMinute = currentMinute % 60f;
        }

        // Обработка перехода дня в ночь и наоборот
        if (isDay && currentHour >= nightStartHour)
        {
            StartNight();
        }
        else if (!isDay && currentHour >= dayStartHour)
        {
            StartDay();
        }

        // Уведомляем подписчиков об изменении времени
        OnTimeChanged?.Invoke(GetHours(), GetMinutes());
    }

    public int GetHours()
    {
        int hours = Mathf.FloorToInt(currentHour) % 24;
        return hours;
    }

    public int GetMinutes()
    {
        return Mathf.FloorToInt(currentMinute);
    }

    public string GetTimeFormatted()
    {
        return string.Format("{0:00}:{1:00}", GetHours(), GetMinutes());
    }

    public void SkipToNight()
    {
        if (isDay)
        {
            currentHour = nightStartHour;
            currentMinute = 0f;
            StartNight();
            UIDebug.Log("День пропущен. Начинается ночь.", Color.yellow);
        }
    }

    private void StartDay()
    {
        isDay = true;
        currentHour = dayStartHour;
        currentMinute = 0f;
        OnDayStart?.Invoke();
        UIDebug.Log("Начался день: " + GetTimeFormatted(), Color.green);
    }

    private void StartNight()
    {
        isDay = false;
        currentHour = 0f; // Полночь = 00:00
        currentMinute = 0f;
        OnNightStart?.Invoke();
        UIDebug.Log("Начались ночь и битва: " + GetTimeFormatted(), Color.blue);
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public bool IsDay()
    {
        return isDay;
    }

    public bool IsNight()
    {
        return !isDay;
    }
}