using UnityEngine;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour
{
    public bool[] enemyInUsed = new bool[4]; // 0: Enemy1, 1: Enemy2, 2: Enemy3, 3: Enemy4
    [SerializeField] private GameObject[] enemyPrefabs = new GameObject[4]; // 0: Enemy1, 1: Enemy2, 2: Enemy3, 3: Enemy4
    [SerializeField] private GameObject[] enemyArrows = new GameObject[4]; // 0: Arrow1, 1: Arrow2, 2: Arrow3, 3: Arrow4

    public GameObject projectilePrefab;
    public float[] enemyDamage = new float[4];
    public float[] fireRate = new float[4]; // every x seconds
    public float[] lastFireTime = new float[4];
    public float[] projectileSpeeds = new float[4];

    public Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Enemy Start");
        CalculatePositions();
    }

    void CalculatePositions(int numLanes = 4)
    {   
        float forwardOffset = 10f;
        Vector3 targetCenter = mainCamera.transform.position + mainCamera.transform.forward * forwardOffset;
        float enemyDistance = 50f;
        Vector3 circleCenter = new Vector3(targetCenter[0], targetCenter[1], targetCenter[2]+enemyDistance);
        // Vector3 targetCenter = playerPlaneLocation.position;
        float tunnelRadius = 10f;

        float angleStep = 360f / numLanes;
        for (int i = 0; i < 4; i++){
            float angle = angleStep * i * Mathf.Deg2Rad;
            enemyPrefabs[i].transform.position = circleCenter + new Vector3(Mathf.Cos(angle) * tunnelRadius, Mathf.Sin(angle) * tunnelRadius, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for testing purpose, press the space key to spawn an enemy
        if (Input.GetKeyDown(KeyCode.Space)) { SpawnEnemy(); }

        // for testing purpose, press the enter key to kill an enemy
        if (Input.GetKeyDown(KeyCode.Return)) {
            List<int> avaliableIndex = new List<int>();
            for (int i = 0; i < enemyInUsed.Length; i++)
            {
                if (enemyInUsed[i])
                {
                    avaliableIndex.Add(i);
                }
            }

            if (avaliableIndex.Count != 0)
            {
                KillEnemy(avaliableIndex[UnityEngine.Random.Range(0, avaliableIndex.Count)]);
            }
        }
        SpawnProjectile();
    }

    public void SpawnEnemy()
    {   
        // Randomly select an enemy to spawn from the available enemies
        List<int> avaliableIndex = new List<int>();
        for (int i = 0; i < enemyInUsed.Length; i++)
        {
            if (!enemyInUsed[i])
            {
                avaliableIndex.Add(i);
            }
        }

        if (avaliableIndex.Count == 0)
        {
            return;
        }

        int enemyIndex = avaliableIndex[UnityEngine.Random.Range(0, avaliableIndex.Count)];

        // update the enemy availability
        enemyInUsed[enemyIndex] = true;

        // display the enemy
        enemyPrefabs[enemyIndex].SetActive(true);

        // Update the arrow text

        // randomly select the arrow text for testing purpose
        string arrows = "";
        for (int i = 0; i < 6; i++) {
            int arrow = UnityEngine.Random.Range(0, 4);
            if (arrow == 0) { arrows += "↑"; }
            else if (arrow == 1) { arrows += "↓"; }
            else if (arrow == 2) { arrows += "←"; }
            else if (arrow == 3) { arrows += "→"; }
        }

        UpdateArrowText(enemyIndex, arrows);
    }

    public void UpdateArrowText(int enemyIndex, string text)
    {   
        // check if the enemyIndex is valid
        if (enemyIndex < 0 || enemyIndex >= enemyInUsed.Length || !enemyInUsed[enemyIndex])
        {   
            Debug.Log("Invalid enemy index");
            return;
        }
        // clear the previous text
        enemyArrows[enemyIndex].GetComponent<TMPro.TextMeshProUGUI>().text = "";

        // find the child object with TextMeshPro - Text component
        enemyArrows[enemyIndex].GetComponent<TMPro.TextMeshProUGUI>().text = text;
        //Debug.Log("Arrow: " + text);
    }

    public void KillEnemy(int enemyIndex)
    {
        // check if the enemyIndex is valid
        if (enemyIndex < 0 || enemyIndex >= enemyInUsed.Length || !enemyInUsed[enemyIndex])
        {
            return;
        }

        // update the enemy availability
        enemyInUsed[enemyIndex] = false;

        // hide the enemy (disable the game object)
        enemyPrefabs[enemyIndex].SetActive(false);
    }

    public void SpawnProjectile()
    {   
        for (int i = 0; i < enemyInUsed.Length; i++)
        {
            if (enemyInUsed[i])
            {
                if (Time.time >= lastFireTime[i]+fireRate[i]){
                    Attack(i);
                    lastFireTime[i] = Time.time;
                }
            }
        }
    }

    void Attack(int enemyLane)
    {
        GameObject projectile = Instantiate(projectilePrefab, enemyPrefabs[enemyLane].transform.position, Quaternion.identity);
        ProjectileMovement projScript = projectile.GetComponent<ProjectileMovement>();
        projScript.projectileDamage = enemyDamage[enemyLane];
        projScript.enemyLane = enemyLane;
        projScript.projectileSpeed = projectileSpeeds[enemyLane];
        projScript.mainCamera = mainCamera;
        // projScript.waypoints = pathWaypoints; // Assign waypoints
    }
}
