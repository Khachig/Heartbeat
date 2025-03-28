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

    void Start()
    {
        enemyManager.init(enemyMovement, audioManager, enemyRhythmManager);
        levelManager.Init(stage, enemyManager, enemyRhythmManager);
        levelManager.StartLevel();
    }

    public void Restart()
    {
        ScoreManager scoreManager = Object.FindAnyObjectByType<ScoreManager>();
        scoreManager.ReInitScore();
    }
}
