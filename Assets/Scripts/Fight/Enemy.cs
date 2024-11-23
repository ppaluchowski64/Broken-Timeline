using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health value
    public int currentHealth;  // Current health value
    public AgeManager ageManager; // Reference to AgeManager

    void Start()
    {
        currentHealth = maxHealth; // Initialize health at maximum
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth; // Restore health to the maximum value
        Debug.Log(gameObject.name + " healed to full! Current health: " + currentHealth);
    }

    void Die()
{
    if (ageManager != null)
    {
        ageManager.ChangeAge(gameObject); // Pass this GameObject as the defeated player
    }
    else
    {
        Debug.LogError("AgeManager is not assigned!");
    }
}
}
