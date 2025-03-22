using UnityEngine;

public class EnemyData : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] public GameObject UpPrefab;
    [SerializeField] public GameObject DownPrefab;
    [SerializeField] public GameObject LeftPrefab;
    [SerializeField] public GameObject RightPrefab;

    [Header("Data")]
    [SerializeField] public float spawnOffsetAngle = Mathf.PI / 6;
    [SerializeField] public float spawnArcRadius = 2.0f;

    [System.Serializable]
    public struct EnemyParameters
    {
        public float spawnOffsetAngle;
        public float spawnArcRadius;
    }

    public void init(EnemyParameters parameters)
    {
        spawnOffsetAngle = parameters.spawnOffsetAngle == 0 ? spawnOffsetAngle : parameters.spawnOffsetAngle;
        spawnArcRadius = parameters.spawnArcRadius == 0 ? spawnArcRadius : parameters.spawnArcRadius;
    }
}
