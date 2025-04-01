using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public bool isGameOver = false;
    private InputAction pauseAction;

    // input action map
    public InputActionAsset inputActions;

    // get end game Canvas
    public GameObject EndGameMenu;

    // get health bar
    public HealthSystem healthSystem;

    void Start()
    {
        healthSystem = GameObject.FindFirstObjectByType<HealthSystem>();
    }

    private void Update()
    {
        if (healthSystem.currentHealth <= 0)
        {   

            ToggleEndGame();
        }
    }
    public void ToggleEndGame()
    {
        isGameOver = !isGameOver;

        if (isGameOver)
        {
            Effects.SpecialEffects.ResetScreenDamageEffect();
            EasyRhythmAudioManager audioManager = Object.FindAnyObjectByType<EasyRhythmAudioManager>();
            GameManager gameManager = Object.FindAnyObjectByType<GameManager>();
            gameManager.Restart();
            audioManager.Reset();

            SceneManager.LoadScene("GameOver");
        }
    }
}