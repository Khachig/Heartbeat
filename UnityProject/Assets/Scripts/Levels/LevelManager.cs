using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private Level firstLevel;

    private bool paused = false;
    private CanvasGroup canvasGroup;
    public GameObject tut1Panel;
    public GameObject tut2Panel;

    public void Init(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager)
    {
        stage = stg;
        enemyManager = eManager;
        enemyRhythmManager = erManager;
        firstLevel = gameObject.AddComponent<EndlessPhasesLevel>();
        firstLevel.onLevelComplete += OnLevelComplete;
        tut1Panel.SetActive(false);
        tut2Panel.SetActive(false);
        // tut1Panel = GameObject.Find("Tut1Panel");
        // tut2Panel = GameObject.Find("Tut2Panel");
        // if (tut1Panel == null || tut2Panel == null){
        //     Debug.Log("find panel failed.");
        // }
    }

    public void StartLevel()
    {
        firstLevel.Load(stage, enemyManager, enemyRhythmManager, tut1Panel, tut2Panel);
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
