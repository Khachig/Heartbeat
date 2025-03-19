using UnityEngine;
using FMODUnity;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    private bool isPaused = false;
    private InputAction pauseAction;

    // input action map
    public InputActionAsset inputActions;

    // get pause menu Canvas
    public GameObject PauseMenu;

    public void OnPause(InputAction.CallbackContext context)
    {   
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            RuntimeManager.PauseAllEvents(true);
            Time.timeScale = 0f;
            // display pause menu
            PauseMenu.gameObject.SetActive(true);
            //focus on the pause menu
            PauseMenu.GetComponentInChildren<UnityEngine.UI.Button>().Select();

        }
        else
        {
            RuntimeManager.PauseAllEvents(false);
            Time.timeScale = 1f;
            // hide pause menu
            PauseMenu.gameObject.SetActive(false);
        }
    }

}
