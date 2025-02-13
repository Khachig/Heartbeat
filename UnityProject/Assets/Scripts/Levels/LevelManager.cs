using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Camera mainCamera;
    private Level firstLevel;

    public void Init(EnemyManager eManager, Camera mCamera)
    {
        enemyManager = eManager;
        mainCamera = mCamera;
        firstLevel = gameObject.AddComponent<DemoLevel>();
        firstLevel.onLevelComplete += OnLevelComplete;
    }

    public void StartLevel()
    {
        firstLevel.Load(enemyManager, mainCamera);
    }

    void OnLevelComplete()
    {
        Debug.Log("Level Complete");
    }

}
