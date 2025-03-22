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
    private float forwardOffset = 80f;

    protected override void Start()
    {
        base.Start();

        if (!playerMovement){
            GameObject playerObject = GameObject.FindWithTag("Player");
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }

        Vector3 spawnPosition = Vector3.down * 5 + Vector3.forward * forwardOffset;
        transform.localPosition = spawnPosition;
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void SpawnNewWave()
    {
        // Spawn new wave of arrows;
        currWave++;
        SetArrowPulsable();
        enemyRhythmManager.InitNewSequence();
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

    Vector3 GetLanePosition(int lane)
    {
        Vector3 newposition = Stage.Lanes.GetXYPosForLane(lane) + Vector3.forward * forwardOffset;
        return newposition;
    }
}
