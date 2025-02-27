using TMPro;  // Required for TextMeshPro
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    //public float maxHealth = 100;
    public float initialHealth = 0;
    private float currentHealth;

    public TextMeshProUGUI healthText;  // Reference to the TextMeshProUGUI component
    // Reference to the health bar slider component
    public UnityEngine.UI.Slider HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial health
        currentHealth = 0;

        // Update the health display
        UpdateHealthText();
        UpdateHealthBar();
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        currentHealth += damage;
        if (currentHealth >= 100) {
            currentHealth = 100;
            Destroy(gameObject);
            }


        // Update the health display
        UpdateHealthText();
        UpdateHealthBar();
    }

    // Method to heal
    public void Heal(float healAmount)
    {
        currentHealth -= healAmount;
        if (currentHealth < initialHealth) currentHealth = initialHealth;

        // Update the health display
        UpdateHealthText();
        UpdateHealthBar();
    }

    // Update the health text display
    private void UpdateHealthText()
    {
        float displayHealth = 100 - currentHealth;
        healthText.text = "Health: " + displayHealth.ToString();
    }

    private void UpdateHealthBar()
    {
        // Calculate the health percentage
        float healthPercentage = currentHealth / 100;
        // Update the health bar
        HealthBar.value = healthPercentage;

        // change the color of the health bar based on the health percentage
        if (healthPercentage > 0.5f)
        {
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
        else if (healthPercentage > 0.25f)
        {
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }
        else
        {
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
    }
}