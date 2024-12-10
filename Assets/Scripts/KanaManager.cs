using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class KanaManager : MonoBehaviour
{
    public List<Kana> KanaList { get; private set; } // The list of kana

    private void Awake()
    {
        LoadKana();
    }

    private void LoadKana()
    {
        // KanaData.json is in Resources / Data / KanaData.json
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/KanaData");

        if (jsonFile != null)
        {
            // Parse the JSON into a list of Kana objects
            KanaList = JsonUtility.FromJson<KanaListWrapper>(jsonFile.text).kana;
            Debug.Log($"Loaded {KanaList.Count} Kana characters successfully.");
        }
        else
        {
            Debug.LogError("KanaData.json not found in Resources/Data");
        }
    }

    // Get a random Kana character from the list
    public Kana GetRandomKana()
    {
        if (KanaList != null && KanaList.Count > 0)
        {
            return KanaList[Random.Range(0, KanaList.Count)];
        }
        else
        {
            Debug.LogWarning("Kana list is empty or not loaded.");
            return null;
        }
    }

    private void Start()
    {
        if (KanaList != null && KanaList.Count > 0)
        {
            Debug.Log($"Loaded {KanaList.Count} Kana characters. First Kana: {KanaList[0].Symbol} ({KanaList[0].Romaji})");
        }
        else
        {
            Debug.LogError("Kana data not loaded properly!");
        }
    }

    [System.Serializable]
    public class KanaListWrapper
    {
        public List<Kana> kana;
    }
}


// Script Purpose: This script is responsible for managing the list of Kana characters in the game. It loads the Kana data from a JSON file and provides methods to retrieve random Kana characters from the list.
// Not removing logging for now, really useful for debugging in Unity in the 2nd Scene;