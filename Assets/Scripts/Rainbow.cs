using TMPro;
using UnityEngine;

public class RainbowText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = -15f;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        text.color = GetRainbowColor(Time.time * speed);
    }

    private Color GetRainbowColor(float time)
    {
        float r = Mathf.Sin(time + 0f) * 0.5f + 0.5f;
        float g = Mathf.Sin(time + 2f) * 0.5f + 0.5f;
        float b = Mathf.Sin(time + 4f) * 0.5f + 0.5f;
        return new Color(r, g, b, 1f); // Always full alpha
    }
}