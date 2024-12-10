using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SRSMode : MonoBehaviour
{
    public TextMeshProUGUI kanaText; // Display the Kana symbol
    public TextMeshProUGUI typeText; // Display the Kana type (Hiragana/Katakana)
    public TextMeshProUGUI timerText; // Display the remaining time for guessing
    public TextMeshProUGUI flashcardsLeftText; // Display remaining flashcards for this session
    public TextMeshProUGUI feedbackText; // Display feedback (Correct, Incorrect, etc.)
    public TMP_InputField inputField; // Input field for user guesses
    public Button submitButton; // Submit answer button
    public Button passButton; // Pass the current card button

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button mainMenuPauseButton;
    private List<LoadUserMastery.UserMastery> userMasteryList; // User mastery data
    private int currentFlashcardIndex = -1; // Index of the current Kana being reviewed
    private int flashcardsLeft = 0; // Total flashcards left for this session
    private float currentTime; // Current countdown time
    private float timeElapsed; // Track time elapsed for the current card
    private bool isPaused = false;

    private void Start()
    {
        // Load user mastery data
        var loadUserMastery = FindObjectOfType<LoadUserMastery>();


        userMasteryList = loadUserMastery.userMasteryList;

        if (userMasteryList == null || userMasteryList.Count == 0)
        {
            Debug.LogError("No UserMastery data available to review!");
            ShowExitMenu();
            return;
        }

        // Filter userMasteryList to include only Kana due for review
        userMasteryList = userMasteryList.FindAll(kana =>
            kana.NextReview == null || kana.NextReview <= System.DateTime.Now);
        // Basically , if the NextReview date is null or in the past, include it

        if (userMasteryList.Count == 0)
        {
            // No Kana due for review
            Debug.Log("No Kana to review today!");
            ShowExitMenu();
            return;
        }

        int cardsToReview = PlayerPrefs.GetInt("CardsToReview", 10);

        // Limit the userMasteryList to the allowed number of cards
        userMasteryList = userMasteryList.GetRange(0, Mathf.Min(cardsToReview, userMasteryList.Count));

        // Initialize session
        flashcardsLeft = userMasteryList.Count;
        flashcardsLeftText.text = $"Flashcards Left: {flashcardsLeft}";
        submitButton.onClick.AddListener(OnSubmit);
        passButton.onClick.AddListener(OnPass);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        mainMenuPauseButton.onClick.AddListener(ReturnToMainMenu);
        resumeButton.onClick.AddListener(ResumeGame);
        ShowNextKana();
    }

    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (currentFlashcardIndex != -1 && !isPaused)
        {
            timeElapsed += Time.deltaTime; // Increment elapsed time
            timerText.text = $"Time Elapsed: {Mathf.Floor(timeElapsed)}s"; // Update UI
        }
    }

    private void ShowNextKana()
    {
        if (flashcardsLeft <= 0)
        {
            // End session
            kanaText.text = "Session complete!";
            typeText.text = "";
            timerText.text = "";
            flashcardsLeftText.text = "No more flashcards.";
            inputField.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
            passButton.gameObject.SetActive(false);
            feedbackText.text = "";

            ShowExitMenu();
            return;
        }

        // Move to the next flashcard
        currentFlashcardIndex = (currentFlashcardIndex + 1) % userMasteryList.Count;
        var currentKana = userMasteryList[currentFlashcardIndex];

        // Reset UI
        kanaText.text = currentKana.KanaSymbol;
        typeText.text = $"Type: {currentKana.Type}";
        inputField.text = "";
        feedbackText.text = "Guess the romaji!";
        timeElapsed = 0f; // Reset elapsed time
        flashcardsLeftText.text = $"Flashcards Left: {flashcardsLeft}";
    }

    private void OnSubmit()
    {
        if (currentFlashcardIndex == -1 || userMasteryList == null) return;

        var currentKana = userMasteryList[currentFlashcardIndex];
        string userAnswer = inputField.text.Trim().ToLower();

        if (userAnswer == currentKana.Romaji.ToLower())
        {
            feedbackText.text = "Correct!";
            UpdateMastery(currentKana, true);
        }
        else
        {
            feedbackText.text = $"Incorrect! The correct answer was: {currentKana.Romaji}";
            UpdateMastery(currentKana, false);
        }

        flashcardsLeft--;
        PlayerPrefs.SetInt("CardsReviewedToday", PlayerPrefs.GetInt("CardsReviewedToday", 0) + 1); // Increment reviewed count
        Invoke(nameof(ShowNextKana), 2f); // Delay before showing the next Kana
    }

    private void OnPass()
    {
        // Skip the current Kana
        feedbackText.text = "You passed. Moving to the next card.";
        flashcardsLeft--;
        Invoke(nameof(ShowNextKana), 2f); // Delay before showing the next Kana
    }

    private void UpdateMastery(LoadUserMastery.UserMastery kana, bool correct)
    {
        // Simple mastery algorithm
        if (correct)
        {
            kana.MasteryLevel = Mathf.Min(kana.MasteryLevel + 10, 100); // Increase mastery by 10, max 100
        }
        else
        {
            kana.MasteryLevel = Mathf.Max(kana.MasteryLevel - 5, 0); // Decrease mastery by 5, min 0
        }

        kana.LastReviewed = System.DateTime.Now;
        kana.NextReview = System.DateTime.Now.AddDays(GetNextReviewInterval(kana.MasteryLevel));

        // Update the database
        CDatabaseConnector.RunProcess($"updateUserMastery {PlayerPrefs.GetInt("LoggedInUserId", 1)} {kana.KanaID} {kana.MasteryLevel} {kana.LastReviewed.Value:yyyy-MM-dd} {kana.NextReview.Value:yyyy-MM-dd}");
    }

    private int GetNextReviewInterval(float masteryLevel)
    {
        // Simple SRS interval algorithm
        if (masteryLevel < 20) return 1; // Review in 1 day
        if (masteryLevel < 50) return 3; // Review in 3 days
        if (masteryLevel < 80) return 7; // Review in 7 days
        return 14; // Review in 14 days
    }

    private void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
    }

    private void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    private void ShowExitMenu()
    {

        gameOverPanel.SetActive(true);
    }

    private void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}