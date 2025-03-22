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
    public EventReference EnemyShoot;
    public EventReference EnemyHurt;
    public EventReference EnemyDefeat;

    protected EnemyRhythmManager enemyRhythmManager;
    protected EnemyData instanceData;
    protected Animator enemyAnimator;
    protected EnemyDamageEffect effects;
    protected List<GameObject> arrows;
    protected bool isDead = false; // To ensure no attacks during death animation;
    protected float timeToJudgementLine = 1f; // How long projectiles should travel before hitting judgement line

    private EasyRhythmAudioManager audioManager;

    protected virtual void Start()
    {
        // this data is per enemy instance
        instanceData = gameObject.GetComponent<EnemyData>();
        effects = gameObject.GetComponent<EnemyDamageEffect>();
        enemyAnimator = gameObject.transform.GetChild(0).GetComponent<Animator>();
        arrows = new List<GameObject>();
    }

    public void Init(EasyRhythmAudioManager aManager, EnemyRhythmManager erManager, float fRate=0)
    {
        enemyRhythmManager = erManager;
        audioManager = aManager;
        audioManager.AddListener(this);
    }

    public void SetTimeToJudgementLine(float ttj)
    {
        timeToJudgementLine = ttj;
    }

    public bool IsDead() { return isDead; }

    public void FlashArrow(int arrowIndex)
    {
        if (arrowIndex >= arrows.Count)
            return;
        
        GameObject arrow = arrows[arrowIndex];
        ArrowFlashEffect arrowEffect = arrow.GetComponent<ArrowFlashEffect>();
        arrowEffect.Flash();
    }

    protected GameObject GetArrowImageFromArrowDirection(ArrowDirection direction)
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

    public void KillEnemy()
    {
        isDead = true;
        onEnemyDestroy?.Invoke();
        // Animator will call destroy on enemy
        enemyAnimator.SetTrigger("EnemyDeath");
        RuntimeManager.PlayOneShot(EnemyDefeat, transform.position);
    }

    public void HitEnemy()
    {
        enemyAnimator.SetTrigger("EnemyHit");
        RuntimeManager.PlayOneShot(EnemyHurt, transform.position);
    }
 
    public void StartAttackAnim()
    {
        enemyAnimator.SetTrigger("EnemyShoot");
    } 
    
    public void StartArrowAttackAnim()
    {
        enemyAnimator.SetTrigger("EnemyShootArrow");
    }

    public virtual void Attack()
    {   
        effects.Flash();
        SpawnProjectile();
        RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
    }

    public virtual void ArrowAttack()
    {   
        effects.Flash();
        SpawnArrowProjectile();
        RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
    }

    protected virtual void SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
        projectile.transform.parent = transform.parent;
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.Init(this, timeToJudgementLine);
    }

    protected virtual void SpawnArrowProjectile()
    {
        ArrowDirection direction = ArrowDirections.GetRandomArrowDirection();
        GameObject arrowProjectilePrefab = GetArrowImageFromArrowDirection(direction);
        GameObject arrowProjectile = Instantiate(arrowProjectilePrefab, gameObject.transform.position, Quaternion.identity);
        arrowProjectile.transform.SetParent(transform.parent);
        ArrowProjectileMovement projScript = arrowProjectile.GetComponent<ArrowProjectileMovement>();
        projScript.Init(this, timeToJudgementLine, direction);
        projScript.onArrowDestroy += (() => { enemyRhythmManager.RemoveArrow(arrowProjectile); });
        enemyRhythmManager.AddArrow(arrowProjectile);
    }

    protected void SetArrowPulsable()
    {
        if (arrows.Count == 0)
            return;

        GameObject arrow = arrows[0];
        Pulsable arrowPulsable = arrow.GetComponent<Pulsable>();
        arrowPulsable.Init(audioManager.myAudioEvent.CurrentTempo / 2f, audioManager);
    }
    
    public void OnBeat(EasyEvent audioEvent)
    {
        // Attack();
        // FlashArrow(audioEvent.CurrentBeat - 1);
    }
}
