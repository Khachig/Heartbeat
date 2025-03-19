using UnityEngine;
using System.Collections;
using System;
public class EnemyMovement : MonoBehaviour, IEasyListener
{
    public PlayerMovement playerMovement; 
    public float moveDuration = 0.3f;
    public float forwardOffset = 20f;
    public float moveInterval = 1f;

    private float lastMove = 0f;
    private GameObject[] activeEnemies;
    public bool enableMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!playerMovement){
            GameObject playerObject = GameObject.FindWithTag("Player");
            playerMovement = playerObject.GetComponent<PlayerMovement>();
        }
        activeEnemies = new GameObject[Stage.Lanes.GetNumLanes()];
        enableMovement = false;
    }

    public void disableEnemyMovement(){
        enableMovement = false;
    }

    public void enableEnemyMovement(){
        enableMovement = true;
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
        GameObject[] newLanePositions = new GameObject[Stage.Lanes.GetNumLanes()]; // New array with the same size
        Array.Copy(activeEnemies, newLanePositions, Stage.Lanes.GetNumLanes());
        for (int i = playerLane-1; i > playerLane-Stage.Lanes.GetNumLanes(); i--){ // goes downwards in lane num, 0->3->2->1, 3->2->1->0
            int currLane = Stage.Lanes.GetModLane(i);
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
                int lowerLane = Stage.Lanes.GetModLane(currLane - 1);
                int upperLane = Stage.Lanes.GetModLane(currLane + 1);
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
        Vector3 newposition = Stage.Lanes.GetXYPosForLane(lane) + Vector3.forward * forwardOffset;
        return newposition;
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (enableMovement == true && Time.time >= lastMove+moveInterval &&
            (audioEvent.CurrentBeat == audioEvent.TimeSigAsArray()[0] / 2 - 1 ||
             audioEvent.CurrentBeat == audioEvent.TimeSigAsArray()[0] - 1)
            )
        {
            moveEnemiesTowardPlayer();
            lastMove = Time.time;
        }
    }
}
