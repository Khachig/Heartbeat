using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: include PlayerManager, LevelManager, whatever is needed etc.
    [SerializeField] private EnemyManager enemyManager;
    public LevelManager levelManager;
    [SerializeField] private Camera mainCamera;

    void Start()
    {
        levelManager.Init(enemyManager, mainCamera);
        levelManager.StartLevel();
    }
}
