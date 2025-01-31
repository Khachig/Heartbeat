using UnityEngine;
using System.Collections.Generic;

public class ArterySegmentGenerator : MonoBehaviour
{
    public GameObject segmentPrefab;
    public float segmentShiftLength;
    public float angleScaling;
    public float generateCooldown;

    private Vector3 currPos;
    private Vector3 prevPos;
    private Vector3 currDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currPos = transform.position;
        currDirection = new Vector3(0, 0, segmentShiftLength);
        InvokeRepeating("GenerateNewSegment", generateCooldown, generateCooldown);
    }

    void GenerateNewSegment() {
        prevPos = currPos;
        currPos += currDirection;
        Vector3 newSegmentPos = Vector3.Lerp(currPos, prevPos, 0.5f);
        Quaternion newSegmentRotate = Quaternion.LookRotation(currDirection) * Quaternion.Euler(90,0,0);
        GameObject newSegment = (GameObject)Instantiate(segmentPrefab, newSegmentPos, newSegmentRotate);

        GenerateNewDirection();
    }

    void GenerateNewDirection() {
        float xCoord = currPos.x;
        float yCoord = currPos.y;

        float newAngleX = Mathf.PerlinNoise(xCoord, yCoord) * GetRandomScaledPosOrNeg();
        float newAngleY = Mathf.PerlinNoise(xCoord + newAngleX, yCoord + newAngleX) * GetRandomScaledPosOrNeg();
        float newAngleZ = Mathf.PerlinNoise(xCoord + newAngleY, yCoord + newAngleY) * GetRandomScaledPosOrNeg();

        Quaternion newRotate = Quaternion.Euler(newAngleX, newAngleY, newAngleZ);
        currDirection = newRotate * currDirection;
    }

    float GetRandomScaledPosOrNeg() {
        return angleScaling * (Random.Range(0,2) * 2 - 1);
    }
}
