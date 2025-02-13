using UnityEngine;
using System.Collections.Generic;

public class ArterySegmentGenerator : MonoBehaviour
{
    // Assumes Prefab is a pipe shape with the "center" at one end of the pipe
    public GameObject segmentPrefab;
    public float segmentShiftLength;
    public float angleScaling;
    public float generateCooldown;
    public int initialTunnelLength = 5; 
    public CameraMovement cameraMovement;

    private Vector3 currPos;
    private Vector3 prevPos;
    private Vector3 currDirection;
    private List<Vector3> points;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = new List<Vector3>();
        currPos = segmentPrefab.transform.position;
        currDirection = new Vector3(0, 0, segmentShiftLength);
        for(int i = 0; i < initialTunnelLength; i++)
            GenerateNewSegment();

        InvokeRepeating("GenerateNewSegment", generateCooldown, generateCooldown);
    }

    void GenerateNewSegment() {
        prevPos = currPos;
        currPos += currDirection;
        Vector3 newSegmentPos = currPos; // Set new position to end of old pipe

        UpdateCameraPath();
        GenerateNewDirection();

        Quaternion newSegmentRotate = Quaternion.LookRotation(currDirection); // Set new rotation for pipe
        GameObject newSegment = (GameObject)Instantiate(segmentPrefab, newSegmentPos, newSegmentRotate);
    }

    void UpdateCameraPath() {
        points.Add(currPos);
        cameraMovement.SetPoints(points);
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
