using UnityEngine;
using UnityEngine.InputSystem;

public class FenceHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100;
    [SerializeField] int dmg = 20;
    public HealthBarTestScript healthBar;
    public int currentHealth;
    

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.SetMaxValue(maxHealth);
        // currentHealth = maxHealth;
        // healthBar.SetMaxValue(maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
        TakeDamage(dmg);
        }

    }
    public void TakeDamage(int dmg)
    {
            currentHealth = Mathf.Max(currentHealth - dmg, 0);
            if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }
    public void Repair(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }
}



