using UnityEngine;

public class KanaMove : MonoBehaviour
{
    public float fallSpeed = 5f;
    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the kana hits the ground, destroy it
        if (other.CompareTag("Ground"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>(); // Find the GameManager in the scene
            if (gameManager != null)
            {
                gameManager.LoseHeart(); // Notify GameManager to reduce hearts
            }

            Destroy(gameObject); // Remove the Kana
        }
    }
}

// Script Purpose - This script is responsible for moving the Kana GameObjects downwards and destroying them when they hit the ground. It also notifies the GameManager to reduce the player's hearts when a Kana reaches the ground.
// The OnTriggerEnter method is used to detect collisions with the ground and trigger the necessary actions.