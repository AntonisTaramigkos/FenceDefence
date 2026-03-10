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
// using System;
// using UnityEngine;

// public class FencePart : MonoBehaviour
// {
//     [SerializeField] private float maxHealth = 50f;
//     private float currentHealth;
//     public HealthBar healthBar;

//     private void Awake()
//     {
//         currentHealth = maxHealth;
//         healthBar.SetMaxHealth(maxHealth);
//     }

//     // ✅ returns true if this hit destroyed the fence
//     public bool TakeDamage(float damage)
//     {
//         currentHealth -= damage;
//         Debug.Log(currentHealth);
//         healthBar.SetHealth(currentHealth);

//         if (currentHealth <= 0f)
//         {
//             Destroy(gameObject);
//             return true;
//         }

//         return false;
//     }
// }


// New With reapair ability 
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
//last working 
// using UnityEngine;

// public class FencePart : MonoBehaviour
// {
//     [Header("Health")]
//     [SerializeField] private float maxHealth = 50f;
//     [SerializeField] private float currentHealth;

//     [Header("Optional UI")]
//     [SerializeField] private HealthBar healthBar;

//     public float MaxHealth => maxHealth;
//     public float CurrentHealth => currentHealth;
//     public bool IsDamaged => currentHealth < maxHealth;
//     public bool IsDestroyed => currentHealth <= 0f;

//     private void Awake()
//     {
//         currentHealth = maxHealth;

//         if (healthBar != null)
//         {
//             healthBar.SetMaxHealth(maxHealth);
//             healthBar.SetHealth(currentHealth);
//         }
//     }

//     // returns true if destroyed
//     public bool TakeDamage(float damage)
//     {
//         if (IsDestroyed) return true;

//         currentHealth = Mathf.Max(0f, currentHealth - damage);

//         if (healthBar != null)
//             healthBar.SetHealth(currentHealth);

//         if (currentHealth <= 0f)
//         {
//             Destroy(gameObject);
//             return true;
//         }

//         return false;
//     }

//     public void Repair(float amount)
//     {
//         if (IsDestroyed) return;

//         currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

//         if (healthBar != null)
//             healthBar.SetHealth(currentHealth);
//     }
// }
//LAst problem try to fix
using UnityEngine;

public class FencePart : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;

    [Header("Broken State")]
    [SerializeField] private bool isBroken;

    [Header("References")]
    [SerializeField] private Collider[] solidColliders;   // colliders that block enemies (NOT player trigger)
    [SerializeField] private GameObject[] visuals;        // meshes / model objects to hide when broken
    public HealthBar healthBar;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public bool IsBroken => isBroken;
    public bool IsDamaged => !isBroken && currentHealth < maxHealth;
    public bool CanBeRepairedOrRebuilt => isBroken || currentHealth < maxHealth;

    private void Awake()
    {
        // If you didn't assign arrays, try auto-find (safe defaults)
        if (solidColliders == null || solidColliders.Length == 0)
            solidColliders = GetComponentsInChildren<Collider>(includeInactive: true);

        if (visuals == null || visuals.Length == 0)
            visuals = new[] { gameObject };

        // Start intact
        currentHealth = maxHealth;
        isBroken = false;

        ApplyState();

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    /// <summary>
    /// Returns true if this hit caused the fence to break.
    /// </summary>
    public bool TakeDamage(float damage)
    {
        if (isBroken) return true;

        damage = Mathf.Max(0f, damage);
        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0f)
        {
            Break();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Repairs if damaged, rebuilds if broken.
    /// </summary>
    public void RepairOrRebuild(float amount)
    {
        amount = Mathf.Max(0f, amount);

        // If broken, "rebuild" by restoring from 0 upward
        if (isBroken)
        {
            isBroken = false;
            currentHealth = 0f;
            ApplyState();
        }

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private void Break()
    {
        isBroken = true;
        currentHealth = 0f;

        ApplyState();

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    private void ApplyState()
    {
        // Visuals off when broken
        if (visuals != null)
        {
            foreach (var v in visuals)
            {
                if (v != null) v.SetActive(!isBroken);
            }
        }

        // Solid colliders disabled when broken so enemies can pass
        if (solidColliders != null)
        {
            foreach (var c in solidColliders)
            {
                // Don't disable triggers (like your player repair trigger, if any exist here)
                if (c != null && !c.isTrigger) c.enabled = !isBroken;
            }
        }
    }
}