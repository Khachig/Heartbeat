using UnityEngine;

public enum ArrowDirection : ushort {
    UP    = 0b_0000_0000_0000_0001,
    DOWN  = 0b_0000_0000_0001_0000,
    LEFT  = 0b_0000_0001_0000_0000,
    RIGHT = 0b_0001_0000_0000_0000,
}

public class EnemyData : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] public GameObject UpPrefab;
    [SerializeField] public GameObject DownPrefab;
    [SerializeField] public GameObject LeftPrefab;
    [SerializeField] public GameObject RightPrefab;

    [Header("Data")]
    /* public Data data; */
    [SerializeField] public int[] arrowArrangement;
    [SerializeField] public float spawnOffsetAngle = Mathf.PI / 6;
    [SerializeField] public float spawnArcRadius = 2.0f;

    /* [System.Serializable] */
    /* public struct Data */
    /* { */
    /*     public int[] arrow_arrangement; */

    /*     public Data(int[] arrangement) */
    /*     { */
    /*         arrow_arrangement = arrangement; */
    /*     } */
    /* } */

    [System.Serializable]
    public struct EnemyParameters
    {
        public int[] arrowArrangement;
        public float spawnOffsetAngle;
        public float spawnArcRadius;
    }

    public void init(EnemyParameters parameters)
    {
        /* data = new Data(arrow_arrangement); */
        arrowArrangement = parameters.arrowArrangement;
        spawnOffsetAngle = parameters.spawnOffsetAngle == 0 ? spawnOffsetAngle : parameters.spawnOffsetAngle;
        spawnArcRadius = parameters.spawnArcRadius == 0 ? spawnArcRadius : parameters.spawnArcRadius;
    }
}
