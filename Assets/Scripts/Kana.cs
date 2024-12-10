using UnityEngine;

[System.Serializable] // This attribute allows the class to be serialized and displayed in the inspector
public class Kana
{
    public string Symbol; // The symbol of the kana
    public string Romaji; // The romaji of the kana
    public string Type; // The type of the kana (Hiragana or Katakana)
}
