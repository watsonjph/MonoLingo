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
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.LoseHeart(); // Notify GameManager to reduce hearts
            }

            Destroy(gameObject); // Remove the Kana
        }
    }
}
