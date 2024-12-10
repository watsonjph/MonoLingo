using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorMessage;
    public Button quitBUtton;


    private void Start()
    {
        quitBUtton.onClick.AddListener(QuitGame);
    }
    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        errorMessage.text = ""; // Clear the error message

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            DisplayError("Username or password cannot be empty.");
            return;
        }

        // Call the C database integration to validate the user
        var loginResult = CDatabaseConnector.LoginUser(username, password);

        if (loginResult.success)
        {
            Debug.Log("Login successful!");

            // Save the username and user data in PlayerPrefs
            PlayerPrefs.SetString("LoggedInUsername", username);
            PlayerPrefs.SetInt("DayStreak", loginResult.dayStreak);
            PlayerPrefs.SetString("LastPracticeDate", loginResult.lastPracticeDate ?? "None");
            PlayerPrefs.Save();

            // Load the main menu
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        else
        {
            DisplayError("Login failed! Invalid username or password.");
        }
    }

    public void CreateUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            DisplayError("Username or password cannot be empty.");
            return;
        }

        // Call the C database integration to create a new user
        bool userCreated = CDatabaseConnector.CreateUser(username, password);

        if (userCreated)
        {
            Debug.Log("User created successfully!");
            DisplaySuccess("User created successfully! Please login to continue.");
        }
        else
        {
            DisplayError("Failed to create user. Please try again.");
        }
    }

    private void DisplayError(string message)
    {
        errorMessage.text = message;
        errorMessage.color = Color.red; // Set the text color to red
    }

    private void DisplaySuccess(string message)
    {
        errorMessage.text = message;
        errorMessage.color = Color.green; // Set the text color to green
    }


    public void QuitGame()
    {
        Application.Quit(); // Quit the game
    }
}
