using UnityEngine;
using System.Collections;
using System;
public class EnemyMovement : MonoBehaviour
{
    public Stage stage;
    public PlayerMovement playerMovement; 
    public float moveDuration = 0.3f;
    public float forwardOffset = 20f;
    public float moveInterval = 1f;

    private float lastMove = 0f;
    private GameObject[] activeEnemies = new GameObject[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!playerMovement){
            GameObject playerObject = GameObject.FindWithTag("Player");
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= lastMove+moveInterval){
            moveEnemiesTowardPlayer();
            lastMove = Time.time;
        }
    }

    public bool addEnemy(int newLane, GameObject newEnemy){
        if (!(activeEnemies[newLane] == null)){
            return false;
        }
        else{
            activeEnemies[newLane] = newEnemy;
            newEnemy.transform.localPosition = GetLanePosition(newLane);
            newEnemy.transform.localRotation = Quaternion.Euler(0, 180, 0);
            return true;
        }
    }
    // does actual movement here
    public void moveEnemiesTowardPlayer(){
        int playerLane = playerMovement.currentLaneIndex;
        GameObject[] newLanePositions = new GameObject[4]; // New array with the same size
        Array.Copy(activeEnemies, newLanePositions, 4);
        for (int i = playerLane-1; i > playerLane-4; i--){ // goes downwards in lane num, 0->3->2->1, 3->2->1->0
            int currLane = ((i % 4) + 4) % 4;
            if (activeEnemies[currLane] == null){
                continue;
            }
            if ((currLane % 2) != (playerLane%2)) // distance 1, move towards
            {
                if (newLanePositions[playerLane]==null){
                    moveEnemyToLane(currLane, playerLane, newLanePositions);
                }
            }
            else if ((currLane % 2) == (playerLane%2)) // distance 2, move to distance 1, going down
            {
                int lowerLane = (((currLane-1) % 4) + 4) % 4;
                int upperLane = (currLane+1) % 4;
                if (newLanePositions[lowerLane] == null){
                    moveEnemyToLane(currLane, lowerLane, newLanePositions);
                    
                }
                else if (newLanePositions[upperLane] == null){
                    moveEnemyToLane(currLane, upperLane, newLanePositions);
                }
            }
        }
        activeEnemies = newLanePositions;
    }

    public void moveEnemyToLane(int prevLane, int newLane, GameObject[] newLanePositions){
        if (activeEnemies[prevLane] == null){
            return;
        }
        GameObject targetEnemy = activeEnemies[prevLane];
        
        newLanePositions[prevLane] = null;
        newLanePositions[newLane] = targetEnemy;
        Transform stageTransform = targetEnemy.transform.parent;
        // Vector3 newposition = GetLanePosition(newLane) + stageTransform.forward * stage.speed * moveDuration;
        Vector3 newposition = GetLanePosition(newLane);
        StartCoroutine(SmoothMove(targetEnemy, newposition));
    }

    IEnumerator SmoothMove(GameObject enemy, Vector3 target)
    {
        Vector3 start = enemy.transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalize time
            enemy.transform.localPosition = Vector3.Lerp(start, target, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        enemy.transform.localPosition = target; // Ensure precise snapping
        yield return null;
    }

    Vector3 GetLanePosition(int lane)
    {
        float angleStep = 360f / stage.numLanes;
        float angle = angleStep * lane * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = newposition.normalized * stage.tunnelRadius + Vector3.forward * forwardOffset;
        return newposition;
    }
    
}
