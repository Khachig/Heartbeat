using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Level firstLevel;

    public void Init(EnemyManager eManager)
    {
        enemyManager = eManager;
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

}
