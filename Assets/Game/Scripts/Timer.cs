using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class Timer : MonoBehaviour
{
    [Header("Настройки")]
    public float dayDurationInSeconds = 60f; // 3 минуты = 180 секунд
    public float dayStartHour = 8f;
    public float dayEndHour = 24f;

    private float timer = 0f;
    private bool isRunning = false;

    [Header("Освещение")]
    public Light2D globalLight; // Ссылка на источник света
    public float startIntensity = 1f; // Интенсивность света в 8:00
    public float endIntensity = 0.2f; // Интенсивность света в 24:00

    [Header("Лампа")]
    public Light2D[] lamps; // Ссылка на лампу
    public float lampStartIntensity = 0f; // Начальная интенсивность лампы
    public float lampEndIntensity = 5f; // Конечная интенсивность лампы

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

        // Обновляем интенсивность света
        UpdateLightIntensity(progress);

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
        globalLight.intensity = 1f;
        foreach (Light2D lamp in lamps)
        {
            lamp.intensity = 0f;
        }

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

    private void UpdateLightIntensity(float progress)
    {
        if (globalLight != null)
        {
            float intensity = Mathf.Lerp(startIntensity, endIntensity, progress);
            globalLight.intensity = intensity;
        }

        // Обновляем интенсивность лампы
        if (lamps.Length > 0)
        {
            foreach (Light2D lamp in lamps)
            {
                float lampIntensity = Mathf.Lerp(lampStartIntensity, lampEndIntensity, progress);
                lamp.intensity = lampIntensity;
            }

        }
    }
}
