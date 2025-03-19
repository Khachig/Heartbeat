using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;

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
            RuntimeManager.PauseAllEvents(true);
            Time.timeScale = 0f;
            // display end game menu
            EndGameMenu.gameObject.SetActive(true);
            //focus on the end game menu
            EndGameMenu.GetComponentInChildren<UnityEngine.UI.Button>().Select();
        }
    }
}