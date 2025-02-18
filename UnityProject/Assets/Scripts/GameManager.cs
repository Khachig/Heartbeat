using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: include PlayerManager, LevelManager, whatever is needed etc.
    [SerializeField] private EnemyManager enemyManager;
    public LevelManager levelManager;

    void Start()
    {
        levelManager.Init(enemyManager);
        levelManager.StartLevel();
    }
}
