using UnityEngine;
using TMPro;

public class GreetUser : MonoBehaviour
{
    public TextMeshProUGUI welcomeText;
    private void Start()
    {
        // Retrieve the username passed from the login screen
        string username = PlayerPrefs.GetString("LoggedInUsername", "User");

        // Display the welcome message
        DisplayWelcomeMessage(username);
    }

    private void DisplayWelcomeMessage(string username)
    {
        welcomeText.text = $"Hi {username}!";
    }
}
