using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using FMODUnity;

public class EnemyBehaviour : MonoBehaviour, IEasyListener
{
    public delegate void OnEnemyDestroy();
    // To be invoked whenever a single enemy is destroyed
    public OnEnemyDestroy onEnemyDestroy;

    public GameObject projectilePrefab;
    public float fireRate = 1f; // every x seconds
    public float lastFireTime = 0f;
    public EventReference EnemyShoot;
    public EventReference EnemyHurt;
    public EventReference EnemyDefeat;

    protected float fireRateMultiplier = 1f;
    protected EnemyRhythmManager enemyRhythmManager;
    protected EnemyData instanceData;
    protected Animator enemyAnimator;
    protected EnemyDamageEffect effects;
    protected List<GameObject> arrows;
    protected bool isDead = false; // To ensure no attacks during death animation;

    private EasyRhythmAudioManager audioManager;

    protected void Start()
    {
        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        effects = gameObject.GetComponent<EnemyDamageEffect>();
        enemyAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        arrows = new List<GameObject>();

        SpawnArrows();
        SetArrowPulsable();

        lastFireTime = Random.Range(0f, 3f);
        fireRate = Random.Range(1f, 3f);
    }

    public void Init(EasyRhythmAudioManager aManager, EnemyRhythmManager erManager)
    {
        enemyRhythmManager = erManager;
        audioManager = aManager;
        audioManager.AddListener(this);
    }

    public void SetFireRateMultiplier(float mult)
    {
        fireRateMultiplier = mult;
    }

    public bool IsDead() { return isDead; }

    // Returns true iff input matches the first arrow of this enemy
    public bool HandlePlayerAttack(Vector2 input)
    {
        if (arrows.Count == 0)
            return true;       

        GameObject nextArrow = arrows[0];
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

    public void FlashArrow(int arrowIndex)
    {
        if (arrowIndex >= arrows.Count)
            return;
        
        GameObject arrow = arrows[arrowIndex];
        ArrowFlashEffect arrowEffect = arrow.GetComponent<ArrowFlashEffect>();
        arrowEffect.Flash();
    }

    protected void SpawnArrows()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();

        for (int i=0; i<instanceData.arrowArrangement.Length; ++i)
        {
            (float X, float Y) spawnCoordinates = GetCoordinatesByIndex(i);
            GameObject arrowPrefab = GetArrowImageFromArrowDirection(instanceData.arrowArrangement[i]);
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.SetParent(canvas.transform, false);

            RectTransform rt = arrow.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(spawnCoordinates.X, spawnCoordinates.Y);

            arrows.Add(arrow);
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
 
    protected virtual void RemoveArrow()
    {
        GameObject arrow = arrows[0];
        arrows.RemoveAt(0);

        SetArrowPulsable();
        effects.Flash();
        Animator arrowAnimator = arrow.GetComponent<Animator>();
        // Animator will call destroy on arrow
        arrowAnimator.SetTrigger("ArrowDestroy");
        // Destroy(arrow, 0.5f);
        ScoreManager.Instance.AddScore(55);
        if (arrows.Count == 0)
        {
            isDead = true;
            onEnemyDestroy?.Invoke();
            enemyRhythmManager.RemoveEnemy(gameObject);
            // Animator will call destroy on enemy
            enemyAnimator.SetTrigger("EnemyDeath");
            RuntimeManager.PlayOneShot(EnemyDefeat, transform.position);

        } else {
            enemyAnimator.SetTrigger("EnemyHit");
            RuntimeManager.PlayOneShot(EnemyHurt, transform.position);
        }
    }

    protected virtual void Attack()
    {   
        if (Time.time >= lastFireTime + fireRate * fireRateMultiplier && !isDead){
            SpawnProjectile();
            RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
            lastFireTime = Time.time;
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
        projectile.transform.parent = transform.parent;
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.Init(this);
    }

    protected void SetArrowPulsable()
    {
        if (arrows.Count == 0)
            return;

        GameObject arrow = arrows[0];
        Pulsable arrowPulsable = arrow.GetComponent<Pulsable>();
        arrowPulsable.Init(audioManager.myAudioEvent.CurrentTempo, audioManager);
    }
    
    public void OnBeat(EasyEvent audioEvent)
    {
        Attack();
        FlashArrow(audioEvent.CurrentBeat - 1);
    }
}
