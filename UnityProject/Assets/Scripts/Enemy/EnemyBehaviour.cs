using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// TODO make arrows a separate class
public class EnemyBehaviour : MonoBehaviour, IEasyListener
{
    public delegate void OnEnemyDestroy();
    // To be invoked whenever a single enemy is destroyed
    public OnEnemyDestroy onEnemyDestroy;

    private EnemyData instanceData;
    private Animator enemyAnimator;
    private EnemyDamageEffect effects;

    private List<GameObject> arrows;

    public float fireRate = 1f; // every x seconds
    public float lastFireTime = 0f;
    public GameObject projectilePrefab;
    public EasyRhythmAudioManager audioManager;
    public EnemyRhythmManager enemyRhythmManager;

    private float bpm;
    private bool needsResetEnemyAnimation = true;
    private bool needsResetArrowAnimation = true;
    private bool isDead = false; // To ensure no attacks during death animation;

    void Start()
    {
        if (!audioManager)
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();
        if (!enemyRhythmManager)
            enemyRhythmManager= GameObject.Find("EnemyRhythmManager").GetComponent<EnemyRhythmManager>();

        audioManager.AddListener(this);
        bpm = audioManager.myAudioEvent.CurrentTempo;

        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        effects = gameObject.GetComponent<EnemyDamageEffect>();
        enemyAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        arrows = new List<GameObject>();

        SpawnArrows();
        SetArrowPulseSpeed();
        SetEnemyPulseSpeed();

        needsResetEnemyAnimation = true;
        needsResetArrowAnimation = true;

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
            if (!enemyRhythmManager.HasBrokenCombo() && enemyRhythmManager.IsNextEnemyInCombo(gameObject))
                enemyRhythmManager.ContinueCombo();
            // Break the combo if the wrong enemy was hit
            else if (!enemyRhythmManager.HasBrokenCombo())
                enemyRhythmManager.BreakCombo();

            RemoveArrow();
            return true;
        }

        return false;
    }

    public void QueueResetEnemyAnim()
    {
        needsResetEnemyAnimation = true;
    }

    // Emphasizes the arrow at arrowIndex based on the original arrow arrangement
    // with label number above arrow
    public void FlashArrow(int arrowIndex, int label)
    {
        int numArrowsDestroyed = instanceData.arrowArrangement.Length - arrows.Count;
        int realIndex = arrowIndex - numArrowsDestroyed;

        if (realIndex < 0)
            return;
        
        GameObject arrow = arrows[realIndex];
        ArrowFlashEffect arrowEffect = arrow.GetComponent<ArrowFlashEffect>();
        arrowEffect.Flash(label);
    }

    void SpawnArrows()
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
 
    void RemoveArrow()
    {
        GameObject arrow = arrows[0];
        arrows.RemoveAt(0);

        SetArrowPulseSpeed();
        effects.Flash();
        Animator arrowAnimator = arrow.GetComponent<Animator>();
        arrowAnimator.SetTrigger("ArrowDestroy");
        if (arrows.Count == 0)
        {
            isDead = true;
            onEnemyDestroy?.Invoke();
            enemyRhythmManager.RemoveEnemy(gameObject);
            enemyAnimator.SetTrigger("EnemyDeath");
        } else {
            enemyAnimator.SetTrigger("EnemyHit");
        }
    }

    void Attack()
    {   
        if (Time.time >= lastFireTime+fireRate && !isDead){
            SpawnProjectile();
            lastFireTime = Time.time;
        }
    }

    void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
        projectile.transform.parent = transform.parent;
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.SetDestroyCallback(this);
        // projScript.projectileDamage = enemyDamage;
    }

    void SetArrowPulseSpeed()
    {
        if (arrows.Count == 0)
            return;

        GameObject arrow = arrows[0];
        Animator arrowAnimator = arrow.GetComponent<Animator>();
        arrowAnimator.SetFloat("ArrowPulseSpeed", bpm / 60f);
        needsResetArrowAnimation = true;
    }

    void SetEnemyPulseSpeed()
    {
        enemyAnimator.SetFloat("EnemyPulseSpeed", bpm / 60f);
    }
    
    public void OnBeat(EasyEvent audioEvent)
    {
        if (needsResetEnemyAnimation)
        {
            enemyAnimator.SetTrigger("Reset");
            needsResetEnemyAnimation = false;
        }
        if (arrows.Count == 0)
            return;

        if (needsResetArrowAnimation)
        {
            GameObject arrow = arrows[0];
            Animator arrowAnimator = arrow.GetComponent<Animator>();
            arrowAnimator.SetTrigger("Reset");
            needsResetArrowAnimation = false;
        }
    }
}
