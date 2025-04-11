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
        
        UpdateHealthBarGraphics(healthPercentage);

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

    private void UpdateHealthBarGraphics(float healthPercentage)
    {
        HealthBar.transform.GetChild(0).gameObject.SetActive(false); // Face for full health
        HealthBar.transform.GetChild(1).gameObject.SetActive(false); // Face for mid health
        HealthBar.transform.GetChild(2).gameObject.SetActive(false); // Face for low health
        HealthBar.transform.GetChild(4).gameObject.SetActive(false); // Heart for full health
        HealthBar.transform.GetChild(5).gameObject.SetActive(false); // Heart for mid health
        HealthBar.transform.GetChild(6).gameObject.SetActive(false); // Heart for low health

        if (healthPercentage > 0.5f)
        {
            HealthBar.transform.GetChild(0).gameObject.SetActive(true); // Face for full health
            HealthBar.transform.GetChild(4).gameObject.SetActive(true); // Heart for full health
        }
        else if (healthPercentage > 0.25f)
        {
            HealthBar.transform.GetChild(1).gameObject.SetActive(true); // Face for mid health
            HealthBar.transform.GetChild(5).gameObject.SetActive(true); // Heart for mid health
        }
        else
        {
            HealthBar.transform.GetChild(2).gameObject.SetActive(true); // Face for low health
            HealthBar.transform.GetChild(6).gameObject.SetActive(true); // Heart for low health
        }
    }

    // reset the health to full
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

}
