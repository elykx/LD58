using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tooltip Style", menuName = "UI/Tooltip Style")]
public class TooltipStyle : ScriptableObject
{
    [Header("Background")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);

    [Header("Title")]
    public Color titleColor = Color.white;
    public int titleFontSize = 18;
    public FontStyles titleFontStyle = FontStyles.Bold;

    [Header("Content")]
    public Color contentColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public int contentFontSize = 14;
    public FontStyles contentFontStyle = FontStyles.Normal;

    [Header("Spacing")]
    public float titleBottomPadding = 5f;
    public int paddingLeft = 10;
    public int paddingRight = 10;
    public int paddingTop = 10;
    public int paddingBottom = 10;
    public RectOffset Padding
    {
        get { return new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom); }
    }
}