using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: include PlayerManager, LevelManager, whatever is needed etc.
    [SerializeField] private EnemyManager enemyManager;

    void Start()
    {
        enemyManager.init();
        enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            position = new Vector3(0, 0, 0),
            rotation = new Quaternion(45, 0, 45, 1),
        });
    }
}
