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

// Script Purpose: This script is responsible for managing the behavior of individual Kana characters in the game.
// It initializes the Kana character with its data and provides a method to retrieve the Romaji representation of the character. The KanaBehavior script is attached to each Kana GameObject in the game scene.