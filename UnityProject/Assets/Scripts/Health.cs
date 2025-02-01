using TMPro;  // Required for TextMeshPro
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;

    public TextMeshProUGUI healthText;  // Reference to the TextMeshProUGUI component

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial health
        currentHealth = maxHealth;

        // Update the health display
        UpdateHealthText();
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) {
            currentHealth = 0;
            Destroy(gameObject);
            }


        // Update the health display
        UpdateHealthText();
    }

    // Method to heal
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Update the health display
        UpdateHealthText();
    }

    // Update the health text display
    private void UpdateHealthText()
    {
        healthText.text = "Health: " + currentHealth.ToString();
    }
}