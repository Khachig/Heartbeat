using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    public delegate void OnEnemyDestroy();
    public OnEnemyDestroy onEnemyDestroy;

    private EnemyData instanceData;

    private int randomOffset;
    private Queue images;

    void Start()
    {
        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        randomOffset = Random.Range(1, 10);
        images = new Queue();

        spawnArrows();

        // TODO: make arrows disappear from player input instead of time
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

        if (spawnCount == 1)
            return (0, spawnRadius);

        float deltaTheta = (Mathf.PI - (2 * spawnOffsetAngle)) / (spawnCount - 1);
        return (
            spawnRadius * Mathf.Cos(spawnOffsetAngle + (deltaTheta * index)),
            spawnRadius * Mathf.Sin(spawnOffsetAngle + (deltaTheta * index))
        );
    }

    public void spawnArrows()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();

        for (int i=0; i<instanceData.arrowArrangement.Length; ++i)
        {
            (float X, float Y) spawnCoordinates = getCoordinatesByIndex(i);
            GameObject imagePrefab = GetArrowImageFromArrowDirection(instanceData.arrowArrangement[i]);
            GameObject image = Instantiate(imagePrefab);
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
            onEnemyDestroy?.Invoke();
            Destroy(gameObject);
            return;
        }
    }

    GameObject GetArrowImageFromArrowDirection(ArrowDirection direction)
    {
        switch (direction)
        {
        case ArrowDirection.UP:
            return instanceData.UpPrefab;
        case ArrowDirection.DOWN:
            return instanceData.DownPrefab;
        case ArrowDirection.LEFT:
            return instanceData.LeftPrefab;
        case ArrowDirection.RIGHT:
            return instanceData.RightPrefab;
        default:
            GameObject[] image_list = {
                instanceData.UpPrefab,
                instanceData.DownPrefab,
                instanceData.LeftPrefab,
                instanceData.RightPrefab
            };
            return image_list[Random.Range(1, 5) % 4];
        }
    }
}
