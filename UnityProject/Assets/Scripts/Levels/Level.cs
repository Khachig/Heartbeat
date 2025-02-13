using UnityEngine;

public interface ILevel
{
    // Call to load the level
    public void Load(EnemyManager enemyManager, Camera mainCamera);
}
