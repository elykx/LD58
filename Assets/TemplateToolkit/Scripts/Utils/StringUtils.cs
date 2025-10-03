using UnityEngine;

public static class StringUtils
{
    /// <summary>
    /// Форматировать большие числа (12345 → "12.3k")
    /// </summary>
    public static string FormatNumber(float number)
    {
        if (number >= 1000000000)
            return (number / 1000000000f).ToString("0.#") + "B";
        if (number >= 1000000)
            return (number / 1000000f).ToString("0.#") + "M";
        if (number >= 1000)
            return (number / 1000f).ToString("0.#") + "k";
        return number.ToString("0");
    }
    
    /// <summary>
    /// Форматировать число с разделителями (12345 → "12,345")
    /// </summary>
    public static string FormatWithSeparator(int number, string separator = ",")
    {
        return number.ToString($"N0").Replace(",", separator);
    }
    
    /// <summary>
    /// Форматировать процент
    /// </summary>
    public static string FormatPercent(float value, int decimals = 0)
    {
        return (value * 100f).ToString($"F{decimals}") + "%";
    }
    
    /// <summary>
    /// Форматировать время (секунды → "MM:SS")
    /// </summary>
    public static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }
    
    /// <summary>
    /// Форматировать время с часами (секунды → "HH:MM:SS")
    /// </summary>
    public static string FormatTimeWithHours(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{hours:00}:{minutes:00}:{secs:00}";
    }
    
    /// <summary>
    /// Сократить строку с многоточием
    /// </summary>
    public static string Truncate(string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;
        return text.Substring(0, maxLength) + suffix;
    }
    
    /// <summary>
    /// Форматировать валюту
    /// </summary>
    public static string FormatCurrency(float amount, string symbol = "$")
    {
        return symbol + FormatNumber(amount);
    }
    
    /// <summary>
    /// Добавить порядковый суффикс (1 → "1st", 2 → "2nd")
    /// </summary>
    public static string AddOrdinalSuffix(int number)
    {
        if (number <= 0) return number.ToString();
        
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }
        
        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }
}