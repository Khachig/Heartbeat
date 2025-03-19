using FMODUnity;
using UnityEngine;

public class TutorialPanelScript : MonoBehaviour
{
    public float displayTime = 15f; // Time before auto-hide
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Pause game while tutorial is active
        Time.timeScale = 0;
        RuntimeManager.PauseAllEvents(true);

        Invoke("HidePanel", displayTime);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            HidePanel();
        }
    }

    void HidePanel()
    {
        canvasGroup.alpha = 0;  // Makes the panel invisible
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Unpause game
        Time.timeScale = 1;
        RuntimeManager.PauseAllEvents(false);
    }
}
