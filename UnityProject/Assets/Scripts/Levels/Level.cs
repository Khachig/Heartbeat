using UnityEngine;

public abstract class Level : MonoBehaviour
{
    public delegate void OnLevelComplete();
    public OnLevelComplete onLevelComplete;

    // Call to load the level
    public abstract void Load(Stage stage, EnemyManager enemyManager, EnemyRhythmManager enemyRhythmManager);
}
