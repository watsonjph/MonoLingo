using TMPro;
using UnityEngine;

public class KanaInputHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameManager gameManager;

    public int scoreValue = 10;
 

    void Start()
    {
        inputField.onEndEdit.AddListener(CheckInput);
    }

    void CheckInput(string userInput)
    {
        // Find all active Kana and check if any match the input
        foreach (KanaBehavior kana in FindObjectsOfType<KanaBehavior>())
        {
            if (kana.GetRomaji().Equals(userInput, System.StringComparison.OrdinalIgnoreCase))
            {
                Destroy(kana.gameObject); // Remove the matched Kana
                inputField.text = ""; // Clear the input field
                inputField.ActivateInputField(); // Keep the field focused

                gameManager.AddScore(scoreValue); // Notify GameManager to increase score
                return;
            }
        }

        inputField.text = ""; // Clear the input field even if no match
        inputField.ActivateInputField();
    }
}
// Script Purpose - This script is responsible for handling user input for Kana characters in the game.
// It checks the user's input against the Romaji representation of active Kana characters in the scene. If a match is found, the corresponding Kana is destroyed, and the player's score is increased. The script is attached to an input field in the game scene.