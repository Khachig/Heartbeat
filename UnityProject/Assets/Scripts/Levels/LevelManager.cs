using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Level firstLevel;

    private bool paused = false;
    private CanvasGroup canvasGroup;

    public void Init(EnemyManager eManager)
    {
        enemyManager = eManager;
        // firstLevel = gameObject.AddComponent<DemoLevel>();
        firstLevel = gameObject.AddComponent<EndlessAlphaLevel>();
        firstLevel.onLevelComplete += OnLevelComplete;
    }

    public void StartLevel()
    {
        firstLevel.Load(enemyManager);
    }

    void OnLevelComplete()
    {
        Debug.Log("Level Complete");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                canvasGroup.alpha = 0;  // Makes the panel invisible
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                // Unpause game
                Time.timeScale = 1;
            }
            else
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // Pause game while tutorial is active
                Time.timeScale = 0;
            }
        }
    }

}
