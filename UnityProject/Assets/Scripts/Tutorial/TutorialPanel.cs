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

        Invoke("HidePanel", displayTime);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HidePanel();
        }
    }

    void HidePanel()
    {
        canvasGroup.alpha = 0;  // Makes the panel invisible
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}