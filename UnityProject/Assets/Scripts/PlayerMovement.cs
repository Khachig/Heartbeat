using UnityEngine;
using System.Collections;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    public Camera mainCamera;
    public int numLanes;
    public int currentLaneIndex = 0;
    public float tunnelRadius;
    private Vector3[] positions;
    public float moveDuration = 0.3f;
    private bool isMoving = false;
    
    private Vector3 circleCenter;
    public float forwardOffset = 10f; 

    public float playerMaxHealth = 100f;
    public float playerCurrentHealth = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!mainCamera) mainCamera = Camera.main; // Auto-assign if not set
        UpdateCircleCenter();
        CalculatePositions();
        transform.position = positions[currentLaneIndex];
    }
    void CalculatePositions()
    {
        positions = new Vector3[numLanes];
        float angleStep = 360f / numLanes;

        for (int i = 0; i < numLanes; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            positions[i] = circleCenter + new Vector3(Mathf.Cos(angle) * tunnelRadius, Mathf.Sin(angle) * tunnelRadius, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Camera.main != null)
        // {
        //     Vector3 camPos = Camera.main.transform.position;
        //     // Debug.Log("Camera Position: " + camPos);
        // }
        // else
        // {
        //     Debug.LogWarning("Main Camera not found!");
        // }
        UpdateCircleCenter();
        CalculatePositions();
        MoveWithCamera();
        if (!isMoving){
            changeLane();
        }
    }
    void UpdateCircleCenter()
    {
        // Set the center of the movement circle slightly in front of the camera
        circleCenter = mainCamera.transform.position + mainCamera.transform.forward * forwardOffset;
        
    }
    void MoveWithCamera()
    {
        // Ensure the player stays at the correct position even when the camera moves
        transform.position = positions[currentLaneIndex];
    }

    void changeLane(){
        if (Input.GetKeyDown(KeyCode.RightArrow)) { 
            currentLaneIndex = (currentLaneIndex + 1) % numLanes;
            StartCoroutine(SmoothMove(positions[currentLaneIndex]));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            currentLaneIndex = (currentLaneIndex - 1);
            if (currentLaneIndex < 0){
                currentLaneIndex = numLanes-1;
            }
            Debug.Log("currentLaneIndex: " + currentLaneIndex);
            StartCoroutine(SmoothMove(positions[currentLaneIndex]));
        }
        
    }

    IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalize time
            t = Mathf.SmoothStep(0, 1, t); // Apply ease-in, ease-out
            transform.position = Vector3.Lerp(start, target, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure precise snapping
        isMoving = false;
    }

    // public void subtractHealth(float damage){
    //     playerCurrentHealth -= damage;
    //     healthText.text = "Health: " + playerCurrentHealth.ToString();
    //     if (playerCurrentHealth <= 0){
    //         Destroy(gameObject);
    //     }
    // }
}