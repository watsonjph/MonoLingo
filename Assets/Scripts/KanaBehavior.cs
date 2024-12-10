using UnityEngine;
using TMPro;

public class KanaBehavior : MonoBehaviour
{
    public TextMeshPro kanaText;
    private Kana kanaData;

    private void Awake()
    {
        // Automatically find the TextMeshPro component on this GameObject
        kanaText = GetComponent<TextMeshPro>();
        
    }

    public void Initialize(Kana kana)
    {
        kanaData = kana;
        kanaText.text = kana.Symbol; // Display the Kana symbol
    }

    public string GetRomaji()
    {
        return kanaData.Romaji;
    }
}
