using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public KanaManager kanaManager;
    public GameObject kanaPrefab;
    public Transform spawnArea;
    public float spawnInterval = 2f; // Time between spawns
    public float spawnOffsetZ = -1f; // Offset distance in front of SpawnArea (negative for in front)
    public int maxHearts = 3;
    private int currentHearts;
    public Transform heartsContainer;
    public Texture fullHeartTexture;
    public Texture emptyHeartTexture;
    public TMPro.TextMeshProUGUI scoreText;

    private BoxCollider spawnAreaCollider;
    private int score;
    public GameObject pauseMenu;
    private bool isPaused = false;

    public GameObject gameOverPanel;
    public TMPro.TextMeshProUGUI finalScoreText;
    public int pointsPerKana;
    public float fallSpeed;


    private void Start()
    {
        // Load difficulty settings
        int selectedDifficulty = PlayerPrefs.GetInt("SelectedDifficulty", 1);

        // Adjust settings based on difficulty
        switch (selectedDifficulty)
        {
            case 0: // Easy
                spawnInterval = 2.5f;
                pointsPerKana = 5;
                fallSpeed = 5f;
                break;
            case 1: // Medium
                spawnInterval = 2.0f;
                pointsPerKana = 10;
                fallSpeed = 10f;
                break;
            case 2: // Hard
                spawnInterval = 1.5f;
                pointsPerKana = 20;
                fallSpeed = 15f;
                break;
        }

        // Attempt to get a BoxCollider from the SpawnArea
        spawnAreaCollider = spawnArea.GetComponent<BoxCollider>();


        currentHearts = maxHearts;
        UpdateHeartsUI();
        UpdateScoreUI();
        InvokeRepeating(nameof(SpawnKana), 0, spawnInterval); // Start spawning Kana
    }

    // SPAHGETTI CODE INCOMING!
    private void Update()
    {
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
    }


    private void SpawnKana()
    {
        Debug.Log("Attempting to spawn Kana...");

        Kana randomKana = kanaManager.GetRandomKana(); // Get a random Kana from the list


        Vector3 spawnPosition = GetRandomSpawnPosition(); // Get a random spawn position



        GameObject kanaGO = Instantiate(kanaPrefab, spawnPosition, Quaternion.identity); // Spawn the Kana


        KanaBehavior kanaBehavior = kanaGO.GetComponent<KanaBehavior>(); // Get the KanaBehavior component
        KanaMove kanaMove = kanaGO.GetComponent<KanaMove>(); // Get the KanaMove component

        kanaMove.fallSpeed = fallSpeed; // Set the fall speed of the Kana
        kanaBehavior.Initialize(randomKana); // Initialize the Kana with the random Kana data
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Spawn Kana in Random X position within the SpawnArea bounds and but with a fixed Z offset

        // Make sure not to spawn in the same position every time

        float randomX = Random.Range(spawnAreaCollider.bounds.min.x, spawnAreaCollider.bounds.max.x);
        float randomY = spawnAreaCollider.bounds.center.y;
        float randomZ = spawnAreaCollider.bounds.center.z + spawnOffsetZ;

        return new Vector3(randomX, randomY, randomZ);
    }

    public void LoseHeart()
    {
        if (currentHearts > 0)
        {
            currentHearts--;
            UpdateHeartsUI();

            if (currentHearts <= 0)
            {
                GameOver();
            }
        }
    }

    public void AddScore(int points)
    {
        score += pointsPerKana;
        UpdateScoreUI();
    }

    private void UpdateHeartsUI()
    {
        // Update each heart image based on the current hearts
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            RawImage heartImage = heartsContainer.GetChild(i).GetComponent<RawImage>(); // Get the RawImage component of the heart
            if (heartImage != null)
            {
                // Show full hearts from 0 to currentHearts-1, empty hearts otherwise
                heartImage.texture = (i < currentHearts) ? fullHeartTexture : emptyHeartTexture; // Set the texture based on the current hearts
            }
        }


    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");

        // Stop Spawning Kana
        CancelInvoke(nameof(SpawnKana));
        int highScore = CDatabaseConnector.GetHighScore(PlayerPrefs.GetString("LoggedInUsername", "Guest"));

        if (score > highScore)
        {
            CDatabaseConnector.UpdateHighScore(PlayerPrefs.GetString("LoggedInUsername", "Guest"), score);

        }

        finalScoreText.text = $"Final Score: {score}\nHigh Score: {highScore}";

        gameOverPanel.SetActive(true);

    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze game time
        pauseMenu.SetActive(true); // Show the Pause Menu
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume game time
        pauseMenu.SetActive(false); // Hide the Pause Menu
    }


}

// Purpose - Manages the game state, spawning Kana, handling player health, score, and game over conditions.