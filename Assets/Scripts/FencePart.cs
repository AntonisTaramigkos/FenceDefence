// using System;
// using UnityEngine;


// public class FencePart : MonoBehaviour
// {
//     [SerializeField] private float maxHealth = 50f;

//     private float currentHealth;

//     private void Awake()
//     {
//         currentHealth = maxHealth;
//     }

//     public void TakeDamage(float damage)
//     {
//         currentHealth -= damage;
//         Debug.Log(currentHealth);

//         if (currentHealth <= 0f)
//         {
//             Destroy(gameObject);
//         }
//     }
// }

//After NavMeshAgent "Bug"
//Correcting the priority of enemies ,flaging the destruction of fence pscs!
using System;
using UnityEngine;

public class FencePart : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    public HealthBar healthBar;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // ✅ returns true if this hit destroyed the fence
    public bool TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
