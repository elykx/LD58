using System;
using System.Collections;
using UnityEngine;

public class TimeUtils : MonoBehaviour
{
    private static TimeUtils instance;
    public static TimeUtils Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("TimeUtils");
                instance = go.AddComponent<TimeUtils>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    public static void DelayedCall(float delay, Action action)
    {
        Instance.StartCoroutine(DelayedCallCoroutine(delay, action));
    }
    
    private static IEnumerator DelayedCallCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    
    public static void DelayedCallRealtime(float delay, Action action)
    {
        Instance.StartCoroutine(DelayedCallRealtimeCoroutine(delay, action));
    }
    
    private static IEnumerator DelayedCallRealtimeCoroutine(float delay, Action action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action?.Invoke();
    }
    
    public static Coroutine Repeat(float interval, Action action, int times = -1)
    {
        return Instance.StartCoroutine(RepeatCoroutine(interval, action, times));
    }
    
    private static IEnumerator RepeatCoroutine(float interval, Action action, int times)
    {
        int count = 0;
        while (times == -1 || count < times)
        {
            action?.Invoke();
            yield return new WaitForSeconds(interval);
            count++;
        }
    }
    
    public static void LerpOverTime(float duration, Action<float> onUpdate, Action onComplete = null)
    {
        Instance.StartCoroutine(LerpOverTimeCoroutine(duration, onUpdate, onComplete));
    }
    
    private static IEnumerator LerpOverTimeCoroutine(float duration, Action<float> onUpdate, Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            onUpdate?.Invoke(t);
            yield return null;
        }
        onUpdate?.Invoke(1f);
        onComplete?.Invoke();
    }
    
    public static void Countdown(float duration, Action<float> onTick, Action onComplete = null)
    {
        Instance.StartCoroutine(CountdownCoroutine(duration, onTick, onComplete));
    }
    
    private static IEnumerator CountdownCoroutine(float duration, Action<float> onTick, Action onComplete)
    {
        float remaining = duration;
        while (remaining > 0)
        {
            onTick?.Invoke(remaining);
            yield return null;
            remaining -= Time.deltaTime;
        }
        onTick?.Invoke(0f);
        onComplete?.Invoke();
    }
}