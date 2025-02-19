using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class EnemyBehaviour : MonoBehaviour
{
    public delegate void OnEnemyDestroy();
    public OnEnemyDestroy onEnemyDestroy;

    private EnemyData instanceData;

    private Queue images;

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
        images = new Queue();

        SpawnArrows();

        lastFireTime = Random.Range(0f, 5f);
        fireRate = Random.Range(1f, 5f);
    }

    void Update()
    {
        PulseEnemy();
        PulseArrow();
        Attack();
    } 

    void OnAttack(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        GameObject nextArrow = (GameObject)images.Peek();
        if ((input.y > 0 && nextArrow.name.Equals("UpArrow(Clone)")) ||
            (input.y < 0 && nextArrow.name.Equals("DownArrow(Clone)")) ||
            // Switch left and right because images are placed "backwards"
            (input.x < 0 && nextArrow.name.Equals("RightArrow(Clone)")) ||
            (input.x > 0 && nextArrow.name.Equals("LeftArrow(Clone)"))
        )
            RemoveArrow();
    }

    void SpawnArrows()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();

        for (int i=0; i<instanceData.arrowArrangement.Length; ++i)
        {
            (float X, float Y) spawnCoordinates = GetCoordinatesByIndex(i);
            GameObject imagePrefab = GetArrowImageFromArrowDirection(instanceData.arrowArrangement[i]);
            GameObject image = Instantiate(imagePrefab);
            image.transform.SetParent(canvas.transform, false);

            RectTransform rt = image.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(spawnCoordinates.X, spawnCoordinates.Y);

            images.Enqueue(image);
        }
    }

    (float X, float Y) GetCoordinatesByIndex(int index)
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

    public void Attack()
    {   
        if (Time.time >= lastFireTime+fireRate){
            SpawnProjectile();
            lastFireTime = Time.time;
        }
    }

    void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        // projScript.projectileDamage = enemyDamage;
    }

    void PulseEnemy()
    {
        gameObject.transform.position = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 0.01f * Mathf.Cos(Time.time * (music.bpm / 60) * Mathf.PI * 2),
            gameObject.transform.position.z
        );
    }

    void PulseArrow()
    {
        GameObject nextArrow = (GameObject) images.Peek();
        float amp = 0.3f;
        float scale = 1f + amp + amp * Mathf.Cos(Time.time * (music.bpm / 60) * Mathf.PI * 2);
        nextArrow.transform.localScale = new Vector3(scale, scale, scale);
    }
}
