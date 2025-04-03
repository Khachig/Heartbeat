using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levels;
    private Stage stage;
    private EasyRhythmAudioManager audioManager;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private PulsableManager pulsabeManager;
    private Level currLevel;
    private int currLevelIdx = 0;
    private bool paused = false;
    private CanvasGroup canvasGroup;

    public void Init(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, EasyRhythmAudioManager aManager, PulsableManager pManager)
    {
        stage = stg;
        enemyManager = eManager;
        enemyRhythmManager = erManager;
        audioManager = aManager;
        pulsabeManager = pManager;
        currLevelIdx = 0;
    }

    private void OnDisable()
    {
        currLevel.onLevelComplete -= OnLevelComplete;
    }

    public void StartNextLevel()
    {
        if (currLevelIdx >= levels.Length)
        {
            audioManager.Reset();
            Debug.Log("Max level reached");
            SceneManager.LoadScene("Win");
            return;
        }

        if (currLevel != null)
        {
            currLevel.onLevelComplete -= OnLevelComplete;
        }

        currLevel = levels[currLevelIdx].GetComponent<Level>();
        currLevel.onLevelComplete += OnLevelComplete;
        currLevel.Load(stage, enemyManager, enemyRhythmManager, audioManager, pulsabeManager);
        currLevelIdx++;
    }

    void OnLevelComplete()
    {
        Debug.Log("Level Complete");
        EndCurLevel();
    }

    void EndCurLevel()
    {
        enemyRhythmManager.KillAllEnemies();
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
