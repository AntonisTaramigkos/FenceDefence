using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth = 30;

    private int currentHealth;
    private bool isDead;

    private WaveManager waveManager;

    private void Awake()   // FIXED (was Wake)
    {
        currentHealth = startingHealth;

        // Cache reference once (better than calling it on every death)
        waveManager = FindObjectOfType<WaveManager>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notify WaveManager
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }
        else
        {
            Debug.LogWarning("WaveManager not found!");
        }

        Destroy(gameObject);
    }
}
