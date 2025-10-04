using UnityEngine;
using UnityEngine.UI;

public class FigureIcon : MonoBehaviour
{
    public Image background;
    public Image icon;

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void SetBackground(Color color)
    {
        background.color = color;
    }
}