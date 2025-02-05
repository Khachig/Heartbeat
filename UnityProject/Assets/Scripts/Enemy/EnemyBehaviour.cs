using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private EnemyData instanceData;

    public void init()
    {
        Debug.Log("INDIVIDUAL ENEMY BEHAVIOUR");

        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();

        instanceData.init(new EnemyData.EnemyParameters {
            arrowArrangement = new int[] {1, 1, 1, 1},
        });

        spawnArrows();
    }

    private (float X, float Y) getCoordinatesByIndex(int index)
    {
        float spawnOffsetAngle = instanceData.spawnOffsetAngle;
        float spawnRadius = instanceData.spawnArcRadius;
        float spawnCount = instanceData.arrowArrangement.Length;

        float deltaTheta = (Mathf.PI - (2 * spawnOffsetAngle)) / (spawnCount - 1);
        /* float spawnRadian = (index * (((2 * Mathf.PI) - (2 * spawnOffsetRadians)) / (arrowCount - 1))); */
        return (
            spawnRadius * Mathf.Cos(spawnOffsetAngle + (deltaTheta * index)),
            spawnRadius * Mathf.Sin(spawnOffsetAngle + (deltaTheta * index))
        );
    }

    public void spawnArrows()
    {
        GameObject[] image_list = {
            instanceData.UpPrefab,
            instanceData.DownPrefab,
            instanceData.LeftPrefab,
            instanceData.RightPrefab
        };

        Canvas canvas = GetComponentInChildren<Canvas>();

        for (int i=0; i<instanceData.arrowArrangement.Length; ++i)
        {
            (float X, float Y) spawnCoordinates = getCoordinatesByIndex(i);
            GameObject image = Instantiate(image_list[i % 4]);
            image.transform.SetParent(canvas.transform, false);

            RectTransform rt = image.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(spawnCoordinates.X, spawnCoordinates.Y);
        }
    }
}
