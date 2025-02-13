using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    public delegate void OnEnemeyDestroy();
    public OnEnemeyDestroy onEnemeyDestroy;

    private EnemyData instanceData;

    private int randomOffset;
    private Queue images;

    void Start()
    {
        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        randomOffset = Random.Range(1, 10);
        images = new Queue();

        instanceData.init(new EnemyData.EnemyParameters {
            arrowArrangement = new int[] {1, 1, 1, 1},
        });

        spawnArrows();

        InvokeRepeating("RemoveArrow", 2 + (2.8f * Random.Range(0, 2)), 0.7f);
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 0.01f * Mathf.Sin(Time.frameCount * 0.05f + randomOffset),
            gameObject.transform.position.z
        );
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

        int randomIndexOffset = Random.Range(1, 5);

        for (int i=0; i<instanceData.arrowArrangement.Length; ++i)
        {
            (float X, float Y) spawnCoordinates = getCoordinatesByIndex(i);
            GameObject image = Instantiate(image_list[(i + randomIndexOffset) % 4]);
            image.transform.SetParent(canvas.transform, false);

            RectTransform rt = image.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(spawnCoordinates.X, spawnCoordinates.Y);

            images.Enqueue(image);
        }
    }

    void RemoveArrow()
    {
        GameObject image = images.Dequeue() as GameObject;
        Destroy(image);
        if (images.Count == 0)
        {
            CancelInvoke();
            onEnemeyDestroy?.Invoke();
            Destroy(gameObject);
            return;
        }
    }
}
