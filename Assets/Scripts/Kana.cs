using UnityEngine;

[System.Serializable] // This attribute allows the class to be serialized and displayed in the inspector
public class Kana
{
    public string Symbol; // The symbol of the kana
    public string Romaji; // The romaji of the kana
    public string Type; // The type of the kana (Hiragana or Katakana)
}

// Purpose - This is a simple data class representing a Kana character in the game. It contains fields for the kana symbol, romaji representation, and type (Hiragana or Katakana). This class is used to store and manage kana data in the game.