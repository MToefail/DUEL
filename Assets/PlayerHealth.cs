using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health & Lives")]
    public float maxHealth = 10f;
    public float currentHealth;
    public int lives = 3;

    [Header("Setup")]
    public Transform respawnPoint;
    public Slider healthSlider;
    public TextMeshProUGUI livesText;

    [Header("Win Screen Setup")]
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private Animator anim;
    private Vector2 startPos;

    void Start()
    {
        currentHealth = maxHealth;
        startPos = transform.position;
        anim = GetComponent<Animator>();
        UpdateUI();
    }

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        UpdateUI();

        if (anim != null) anim.SetTrigger("Hurt");

        if (currentHealth <= 0) HandleDeath();
    }

    void HandleDeath()
    {
        lives--;
        if (lives > 0)
        {
            Respawn();
        }
        else
        {
            GameOver();
        }
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = respawnPoint != null ? respawnPoint.position : startPos;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        UpdateUI();
    }
    void GameOver()
    {
        gameObject.SetActive(false);
        if (winPanel != null) winPanel.SetActive(true);

        if (winText != null)
        {
            if (gameObject.name == "P1") winText.text = "PLAYER 2 WINS!";
            else if (gameObject.name == "player 2") winText.text = "PLAYER 1 WINS!";
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (livesText != null) livesText.text = "Lives: " + lives;
    }
}
