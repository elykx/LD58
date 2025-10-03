using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    public static float Clamp(float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    public static int Clamp(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }

    public static T RandomFromArray<T>(T[] array)
    {
        if (array == null || array.Length == 0)
            return default(T);
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T RandomFromList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T RandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public static float SmoothLerp(float current, float target, float smoothTime)
    {
        return Mathf.Lerp(current, target, 1f - Mathf.Exp(-smoothTime * Time.deltaTime));
    }

    public static bool InRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }

    public static float RoundToNearest(float value, float multiple)
    {
        return Mathf.Round(value / multiple) * multiple;
    }

    public static float Percentage(float value, float total)
    {
        if (total == 0) return 0;
        return (value / total) * 100f;
    }
}