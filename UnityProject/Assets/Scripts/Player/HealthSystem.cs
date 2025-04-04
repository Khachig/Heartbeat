using TMPro;  // Required for TextMeshPro
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public EnemyRhythmManager EnemyRhythmManager;

    // Reference to the health bar slider component
    public UnityEngine.UI.Slider HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        // Set the initial health
        currentHealth = maxHealth;

        // Update the health display
        UpdateHealthBar();
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        //reset combo
        EnemyRhythmManager.BreakCombo();
        EnemyRhythmManager.ResetCombo();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //Destroy(gameObject);
        }

        // Update the health display
        UpdateHealthBar();
    }

    // Method to heal
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // Update the health display
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // Calculate the health percentage
        float healthPercentage = currentHealth / maxHealth;
        // Update the health bar
        HealthBar.value = healthPercentage;

        // change the color of the health bar based on the health percentage
        if (healthPercentage > 0.5f)
        {
            Color newColor;
            ColorUtility.TryParseHtmlString("#FF00FF", out newColor);
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = newColor;
        }
        else if (healthPercentage > 0.25f)
        {
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        }
        else
        {
            HealthBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        }
    }

    // reset the health to full
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

}
