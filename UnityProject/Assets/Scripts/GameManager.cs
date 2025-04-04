using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: include PlayerManager, LevelManager, whatever is needed etc.
    [SerializeField] private EnemyManager enemyManager;
    public LevelManager levelManager;
    public Stage stage;
    public EnemyMovement enemyMovement;
    public EasyRhythmAudioManager audioManager;
    public EnemyRhythmManager enemyRhythmManager;
    public PulsableManager pulsableManager;

    void Start()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.FindAnyObjectByType<EasyRhythmAudioManager>();
        }
        enemyManager.init(enemyMovement, audioManager, enemyRhythmManager);
        levelManager.Init(stage, enemyManager, enemyRhythmManager, audioManager, pulsableManager);
        levelManager.StartNextLevel();
    }

    public void Restart()
    {
        ScoreManager scoreManager = Object.FindAnyObjectByType<ScoreManager>();
        scoreManager.ReInitScore();
    }
}
