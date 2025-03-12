using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using FMODUnity;

public class BossBehaviour : EnemyBehaviour, IEasyListener
{
    public int numWaves = 3;
    public float burstRate = 0.3f;
    public float sprayRate = 0.3f;

    private PlayerMovement playerMovement; 
    private int currWave = 1;
    private Coroutine currRoutine;
    private float forwardOffset = 30f;

    protected override void Start()
    {
        base.Start();

        if (!playerMovement){
            GameObject playerObject = GameObject.FindWithTag("Player");
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }

        lastFireTime = 3f;
        fireRate = 3f;

        Vector3 spawnPosition = Vector3.down * 5 + Vector3.forward * forwardOffset;
        transform.localPosition = spawnPosition;
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    protected override void RemoveArrow()
    {
        GameObject arrow = arrows[0];
        arrows.RemoveAt(0);

        SetArrowPulsable();
        effects.Flash();
        Animator arrowAnimator = arrow.GetComponent<Animator>();
        // Animator will call destroy on arrow
        arrowAnimator.SetTrigger("ArrowDestroy");
        if (arrows.Count == 0)
        {
            if (currWave >= numWaves)
            {
                isDead = true;
                onEnemyDestroy?.Invoke();
                enemyRhythmManager.RemoveEnemy(gameObject);
                // Animator will call destroy on enemy
                enemyAnimator.SetTrigger("EnemyDeath");
                RuntimeManager.PlayOneShot(EnemyDefeat, transform.position);
            }
            else
            { 
                Invoke("SpawnNewWave", 2);
                enemyAnimator.SetTrigger("EnemyHit");
                RuntimeManager.PlayOneShot(EnemyDefeat, transform.position);
            }
        } else {
            enemyAnimator.SetTrigger("EnemyHit");
            RuntimeManager.PlayOneShot(EnemyHurt, transform.position);
        }
    }

    void SpawnNewWave()
    {
        // Spawn new wave of arrows;
        currWave++;
        instanceData.arrowArrangement = GetRandomArrowArrangement(4);
        SpawnArrows();
        SetArrowPulsable();
        enemyRhythmManager.InitNewSequence();
    }

    ArrowDirection[] GetRandomArrowArrangement(int numArrows)
    {
        ArrowDirection[] newList = new ArrowDirection[numArrows];
        for (int i = 0; i < numArrows; i++)
            newList[i] = ArrowDirection.RANDOM;
        return newList;
    }

    public override void Attack()
    {   
        int r = Random.Range(0, 3);
        if (r == 0)
        {
            currRoutine = StartCoroutine("BurstFireRoutine");
        }
        else if (r == 1)
        {
            currRoutine = StartCoroutine("SprayFireRoutine");
        }
        else
        {
            NeedleFire();
        }
    }

    IEnumerator BurstFireRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnProjectile(playerMovement.currentLaneIndex);
            RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
            yield return new WaitForSeconds(burstRate);
        }
        currRoutine = null;
    }

    IEnumerator SprayFireRoutine()
    {
        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, Stage.Lanes.GetNumLanes());
            SpawnProjectile(r);
            RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
            yield return new WaitForSeconds(sprayRate);
        }
        currRoutine = null;
    }
    
    void NeedleFire()
    {
        int r = Stage.Lanes.GetModLane(Random.Range(-1, 2) + playerMovement.currentLaneIndex); 

        for (int i = 0; i < 3; i++)
        {
            SpawnProjectile((r + i) % Stage.Lanes.GetNumLanes());
        }
        RuntimeManager.PlayOneShot(EnemyShoot, transform.position);
    }

    void SpawnProjectile(int lane)
    {
        Vector3 pos = GetLanePosition(lane);
        GameObject projectile = Instantiate(projectilePrefab, pos, Quaternion.identity);
        projectile.transform.parent = transform.parent;
        projectile.transform.localPosition = pos;
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.Init(this, timeToJudgementLine);
    }

    protected override void SpawnArrowProjectile()
    {
        Vector3 pos = GetLanePosition(playerMovement.currentLaneIndex);
        GameObject arrowProjectilePrefab = GetArrowImageFromArrowDirection(ArrowDirection.RANDOM);
        GameObject arrowProjectile = Instantiate(arrowProjectilePrefab, pos, Quaternion.identity);
        arrowProjectile.transform.SetParent(transform.parent);
        arrowProjectile.transform.localPosition = pos;
        ArrowProjectileMovement projScript = arrowProjectile.GetComponent<ArrowProjectileMovement>();
        projScript.Init(this, timeToJudgementLine);
        projScript.onArrowDestroy += (() => { enemyRhythmManager.RemoveArrow(arrowProjectile); });
        enemyRhythmManager.AddArrow(arrowProjectile);
    }

    Vector3 GetLanePosition(int lane)
    {
        Vector3 newposition = Stage.Lanes.GetXYPosForLane(lane) + Vector3.forward * forwardOffset;
        return newposition;
    }
}
