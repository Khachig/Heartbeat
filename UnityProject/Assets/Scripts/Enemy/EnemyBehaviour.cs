using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    public delegate void OnEnemyDestroy();
    public OnEnemyDestroy onEnemyDestroy;

    private EnemyData instanceData;

    private int randomOffset;
    private Queue images;
    public int enemyLane;

    public float fireRate = 1f; // every x seconds
    public float lastFireTime = 0f;
    public GameObject projectilePrefab;
    public Music music;

    void Start()
    {
        if (!music)
            music = GameObject.Find("Music").GetComponent<Music>();
        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        randomOffset = Random.Range(1, 10);
        images = new Queue();

        spawnArrows();

        lastFireTime = Random.Range(0f, 5f);
        fireRate = Random.Range(1f, 5f);
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 0.01f * Mathf.Cos(Time.time * (music.bpm / 60) * Mathf.PI * 2),
            gameObject.transform.position.z
        );
        PulseArrow();

        HandlePlayerInput();
        Attack();
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

    void HandlePlayerInput()
    {
        GameObject nextArrow = (GameObject) images.Peek();
        if ((Input.GetKeyDown(KeyCode.UpArrow) && nextArrow.name.Equals("UpArrow(Clone)")) ||
            (Input.GetKeyDown(KeyCode.DownArrow) && nextArrow.name.Equals("DownArrow(Clone)")) || 
            // Switch left and right because images are placed "backwards"
            (Input.GetKeyDown(KeyCode.LeftArrow) && nextArrow.name.Equals("RightArrow(Clone)")) ||
            (Input.GetKeyDown(KeyCode.RightArrow) && nextArrow.name.Equals("LeftArrow(Clone)"))
        )
        {
            RemoveArrow();
        }
    }

    public void Attack()
    {   
        if (Time.time >= lastFireTime+fireRate){
            SpawnProjectile();
            lastFireTime = Time.time;
        }
    }
    // int enemyLane
    void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        // projScript.projectileDamage = enemyDamage;
    }

    void PulseArrow()
    {
        GameObject nextArrow = (GameObject) images.Peek();
        float amp = 0.3f;
        float scale = 1f + amp + amp * Mathf.Cos(Time.time * (music.bpm / 60) * Mathf.PI * 2);
        nextArrow.transform.localScale = new Vector3(scale, scale, scale);
    }
}
