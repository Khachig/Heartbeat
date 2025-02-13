using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private EnemyManager enemyManager;
    private Camera mainCamera;
    private ILevel firstLevel;

    public void Init(EnemyManager eManager, Camera mCamera)
    {
        enemyManager = eManager;
        mainCamera = mCamera;
        firstLevel = gameObject.AddComponent<DemoLevel>();
    }

    public void StartLevel()
    {
        firstLevel.Load(enemyManager, mainCamera);
    }
}
