using System.Collections.Generic;
using UnityEngine;

public class LoadUserMastery : MonoBehaviour
{
    public List<UserMastery> userMasteryList = new List<UserMastery>();
    private int userId = 1; // Replace with the logged-in user ID

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Prevent this GameObject from being destroyed
        Load();
    }

    void Start()
    {
        Load();
    }

    private void Load()
    {
        string result = CDatabaseConnector.RunProcess($"getUserMastery {userId}");
        Debug.Log(result);

        if (result.StartsWith("success"))
        {
            string[] lines = result.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("success"))
                {
                    string[] parts = line.Split('|'); // Split the line into parts

                    // Add safety checks for NULL or invalid date values
                    System.DateTime? lastReviewed = null;
                    if (!string.IsNullOrEmpty(parts[6]) && parts[6] != "None") // Check if the last review date is not empty or None
                    {
                        if (System.DateTime.TryParse(parts[6], out var parsedLastReviewed))
                        {
                            lastReviewed = parsedLastReviewed;
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to parse LastReviewed: {parts[6]}");
                        }
                    }

                    System.DateTime? nextReview = null;
                    if (!string.IsNullOrEmpty(parts[7]) && parts[7] != "None")
                    {
                        if (System.DateTime.TryParse(parts[7], out var parsedNextReview))
                        {
                            nextReview = parsedNextReview;
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to parse NextReview: {parts[7]}");
                        }
                    }

                    userMasteryList.Add(new UserMastery
                    {
                        KanaID = int.Parse(parts[1]),
                        KanaSymbol = parts[2],
                        Romaji = parts[3],
                        Type = parts[4],
                        MasteryLevel = float.Parse(parts[5]),
                        LastReviewed = lastReviewed,
                        NextReview = nextReview
                    });
                }
            }

        }
        else
        {
            // Had to add this, UserMastery was a fucking pain to debug foafjkqwepofj ://
            Debug.LogError("Failed to load UserMastery data.");
        }
    }

    // Simple data class for UserMastery
    public class UserMastery
    {
        public int KanaID;
        public string KanaSymbol;
        public string Romaji;
        public string Type;
        public float MasteryLevel;
        public System.DateTime? LastReviewed;
        public System.DateTime? NextReview;
    }
}

// Script Purpose - Load user mastery data from the database and store it in a list of UserMastery objects. The UserMastery class represents the data structure for each entry in the user's mastery list, including the kana ID, symbol, romaji, type, mastery level, last reviewed date, and next review date.