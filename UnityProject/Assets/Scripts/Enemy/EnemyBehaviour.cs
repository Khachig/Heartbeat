using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class EnemyBehaviour : MonoBehaviour
{
    public delegate void OnEnemyDestroy();
    // To be invoked whenever a single enemy is destroyed
    public OnEnemyDestroy onEnemyDestroy;

    private EnemyData instanceData;
    private Animator enemyAnimator;
    private EnemyDamageEffect effects;

    private Queue images;

    public float fireRate = 1f; // every x seconds
    public float lastFireTime = 0f;
    public GameObject projectilePrefab;
    public EasyRhythmAudioManager audioManager;

    private float bpm;

    void Start()
    {
        if (!audioManager)
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();

        bpm = audioManager.myAudioEvent.CurrentTempo;
        BeatCapturer.onBeatCapture += OnBeatCaptured;

        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        effects = gameObject.GetComponent<EnemyDamageEffect>();
        enemyAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        images = new Queue();

        SpawnArrows();
        SetArrowPulseSpeed();
        SetEnemyPulseSpeed();

        lastFireTime = Random.Range(0f, 5f);
        fireRate = Random.Range(1f, 5f);
    }

    void Update()
    {
        Attack();
    } 

    // Returns true iff input matches the first arrow of this enemy
    public bool HandlePlayerAttack(Vector2 input)
    {
        if (images.Count == 0)
            return true;       

        GameObject nextArrow = (GameObject)images.Peek();
        if ((input.y > 0 && nextArrow.name.Equals("UpArrow(Clone)")) ||
            (input.y < 0 && nextArrow.name.Equals("DownArrow(Clone)")) ||
            // Switch left and right because images are placed "backwards"
            (input.x < 0 && nextArrow.name.Equals("RightArrow(Clone)")) ||
            (input.x > 0 && nextArrow.name.Equals("LeftArrow(Clone)"))
        )
        {
            RemoveArrow();
            return true;
        }
        return false;
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
        SetArrowPulseSpeed();
        effects.Flash();
        Animator arrowAnimator = image.GetComponent<Animator>();
        arrowAnimator.SetTrigger("ArrowDestroy");
        if (images.Count == 0)
        {
            onEnemyDestroy?.Invoke();
            enemyAnimator.SetTrigger("EnemyDeath");
        } else {
            enemyAnimator.SetTrigger("EnemyHit");
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

    void SetArrowPulseSpeed()
    {
        if (images.Count == 0)
            return;

        GameObject image = (GameObject) images.Peek();
        Animator arrowAnimator = image.GetComponent<Animator>();
        arrowAnimator.SetFloat("ArrowPulseSpeed", bpm / 60f);
        arrowAnimator.SetBool("IsActive", true);
    }

    void SetEnemyPulseSpeed()
    {
        enemyAnimator.SetFloat("EnemyPulseSpeed", bpm / 60f);
    }

    void OnBeatCaptured()
    {
        // Debug.Log("bpm: " + bpm);
    }
}
