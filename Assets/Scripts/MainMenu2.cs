using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu2 : MonoBehaviour
{
    public TextMeshProUGUI practiceInfoText;
    private string loggedInUsername;
    private string todayDate;
    private int currentStreak;
    public TMP_InputField cardsToReviewInput;
    public TextMeshProUGUI errorMessageText; // Error message if input is invalid
    public Button startSRSButton;
    public TMP_Dropdown difficultyDropdown;
    public Button playGameButton;
    public TextMeshProUGUI highScoreText;
    public Button quitButton;
    private void Start()
    {
        loggedInUsername = PlayerPrefs.GetString("LoggedInUsername", "Guest");

        // Fetch practice info from the database
        var practiceInfo = CDatabaseConnector.GetPracticeInfo(loggedInUsername);
        string lastPracticeDate = practiceInfo.lastPracticeDate;
        int dayStreak = practiceInfo.dayStreak;

        // Display current streak and last practice date
        UpdatePracticeInfoUI(lastPracticeDate, dayStreak);
        int savedCardsToReview = PlayerPrefs.GetInt("CardsToReview", 10);
        difficultyDropdown.value = PlayerPrefs.GetInt("SelectedDifficulty", 1); // Default to Medium
        cardsToReviewInput.text = savedCardsToReview.ToString();
        startSRSButton.onClick.AddListener(PlaySRS);
        playGameButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
        int highScore = CDatabaseConnector.GetHighScore(loggedInUsername);
        highScoreText.text = $"High Score: {highScore}";
    }

    public void PlayGame()
    {
        // Save the selected difficulty
        PlayerPrefs.SetInt("SelectedDifficulty", difficultyDropdown.value);
        // Update the streak
        todayDate = System.DateTime.Now.ToString("yyyy-MM-dd");

        // Fetch current streak info
        var practiceInfo = CDatabaseConnector.GetPracticeInfo(loggedInUsername);
        string lastPracticeDate = practiceInfo.lastPracticeDate;
        int dayStreak = practiceInfo.dayStreak;

        // Calculate and update streak
        if (!string.IsNullOrEmpty(lastPracticeDate) && lastPracticeDate != "None")
        {
            System.DateTime lastDate = System.DateTime.Parse(lastPracticeDate).Date;
            System.DateTime currentDate = System.DateTime.Now.Date;

            int daysDifference = (currentDate - lastDate).Days;

            if (daysDifference == 1)
            {
                dayStreak += 1; // Continue streak
            }
            else if (daysDifference > 1)
            {
                dayStreak = 1; // Reset streak
            }
        }
        else
        {
            dayStreak = 1; // First practice or no previous date
        }

        // Update the database
        bool updateSuccessful = CDatabaseConnector.UpdatePracticeInfo(loggedInUsername, todayDate, dayStreak);

        if (updateSuccessful)
        {
            // Update the UI with the new streak
            UpdatePracticeInfoUI(todayDate, dayStreak);
        }
        else
        {
            Debug.LogError("Failed to update practice info.");
        }


        // Load the next scene (e.g., game scene)
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void PlaySRS()
    {
        // Validate the number of cards to review
        if (int.TryParse(cardsToReviewInput.text, out int cardsToReview) && cardsToReview > 0)
        {
            PlayerPrefs.SetInt("CardsToReview", cardsToReview); // Save the user's selection
            UnityEngine.SceneManagement.SceneManager.LoadScene("SRSMode"); // Proceed to SRS scene
        }
        else
        {
            errorMessageText.text = "Please enter a valid number of cards (greater than 0).";
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the game
    }

    private void UpdatePracticeInfoUI(string lastPracticeDate, int dayStreak)
    {
        string displayDate = string.IsNullOrEmpty(lastPracticeDate) || lastPracticeDate == "None" ? "None" : lastPracticeDate;
        practiceInfoText.text = $"Last Practice: {displayDate}\nStreak: {dayStreak} days";
    }
}