using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// TODO make arrows a separate class
public class BossBehaviour : EnemyBehaviour, IEasyListener
{
    public PlayerMovement playerMovement; 
    public Stage stage;
    public int numWaves = 3;
    public float burstRate = 0.1f;
    public float sprayRate = 0.2f;
    private int currWave = 1;

    private Coroutine currRoutine;


    void Start()
    {
        base.Start();

        if (!playerMovement){
            GameObject playerObject = GameObject.FindWithTag("Player");
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }

        lastFireTime = 3f;
        fireRate = 3f;

        Vector3 spawnPosition = Vector3.down * 5 + Vector3.forward * 25;
        transform.localPosition = spawnPosition;
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public void SetStage(Stage stg)
    {
        stage = stg;
    }

    void Update()
    {
        Attack();
    } 
 
    protected override void RemoveArrow()
    {
        GameObject arrow = arrows[0];
        arrows.RemoveAt(0);

        SetArrowPulseSpeed();
        effects.Flash();
        Animator arrowAnimator = arrow.GetComponent<Animator>();
        arrowAnimator.SetTrigger("ArrowDestroy");
        if (arrows.Count == 0)
        {
            if (currWave >= numWaves)
            {
                isDead = true;
                onEnemyDestroy?.Invoke();
                enemyRhythmManager.RemoveEnemy(gameObject);
                enemyAnimator.SetTrigger("EnemyDeath");
            }
            else
            { 
                Invoke("SpawnNewWave", 2);
                enemyAnimator.SetTrigger("EnemyHit");
            }
        } else {
            enemyAnimator.SetTrigger("EnemyHit");
        }
    }

    void SpawnNewWave()
    {
        // Spawn new wave of arrows;
        currWave++;
        instanceData.arrowArrangement = GetRandomArrowArrangement(4);
        SpawnArrows();
        SetArrowPulseSpeed();
        needsResetArrowAnimation = true;
        enemyRhythmManager.InitNewSequence();
    }

    ArrowDirection[] GetRandomArrowArrangement(int numArrows)
    {
        ArrowDirection[] newList = new ArrowDirection[numArrows];
        for (int i = 0; i < numArrows; i++)
            newList[i] = ArrowDirection.RANDOM;
        return newList;
    }

    void Attack()
    {   
        if (Time.time >= lastFireTime + fireRate * fireRateMultiplier && !isDead && currRoutine == null){
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

            lastFireTime = Time.time;
        }
    }

    IEnumerator BurstFireRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            SpawnProjectile(playerMovement.currentLaneIndex);
            yield return new WaitForSeconds(burstRate);
        }
        currRoutine = null;
    }

    IEnumerator SprayFireRoutine()
    {
        for (int i = 0; i < 6; i++)
        {
            int r = Random.Range(0, stage.numLanes);
            SpawnProjectile(r);
            yield return new WaitForSeconds(sprayRate);
        }
        currRoutine = null;
    }
    
    void NeedleFire()
    {
        int r = Random.Range(0, stage.numLanes);
        for (int i = 0; i < 3; i++)
        {
            SpawnProjectile((r + i) % stage.numLanes);
        }
    }

    void SpawnProjectile(int lane)
    {
        Vector3 pos = GetLanePosition(lane);
        GameObject projectile = Instantiate(projectilePrefab, pos, Quaternion.identity);
        projectile.transform.parent = transform.parent;
        projectile.transform.localPosition = pos;
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.SetDestroyCallback(this);
    }

    Vector3 GetLanePosition(int lane)
    {
        float angleStep = 360f / stage.numLanes;
        float angle = angleStep * lane * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = newposition.normalized * stage.tunnelRadius + Vector3.forward * 25f;
        return newposition;
    }
}
