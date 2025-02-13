using UnityEngine;
using System.Collections;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    public CameraMovement cameraMovement;
    public int numLanes;
    public int currentLaneIndex = 0;
    public float tunnelRadius;
    public float moveDuration = 0.3f;
    private bool isMoving = false;
    
    private Vector3 circleCenter;
    public float forwardOffset = 10f; 

    public float playerMaxHealth = 100f;
    public float playerCurrentHealth = 90f;

    private float angleStep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCircleCenter();
        MoveWithCamera();
        angleStep = 360f / numLanes;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCircleCenter();
        MoveWithCamera();
        if (!isMoving){
            ChangeLane();
        }
    }
    void UpdateCircleCenter()
    {
        // Set the center of the movement circle slightly in front of the camera
        circleCenter = cameraMovement.transform.position + cameraMovement.transform.forward * forwardOffset;
    }

    void MoveWithCamera()
    {
        // Ensure the player stays at the correct position even when the camera moves
        transform.position = GetCurrPosition();
    }

    void ChangeLane(){
        if (Input.GetKeyDown(KeyCode.D)) { 
            currentLaneIndex = (currentLaneIndex + 1) % numLanes;
            Vector3 newposition = GetCurrPosition() + cameraMovement.transform.forward * cameraMovement.speed * moveDuration;
            StartCoroutine(SmoothMove(newposition));
        }
        else if (Input.GetKeyDown(KeyCode.A)){
            currentLaneIndex = (currentLaneIndex - 1);
            if (currentLaneIndex < 0){
                currentLaneIndex = numLanes-1;
            }
            Vector3 newposition = GetCurrPosition() + cameraMovement.transform.forward * cameraMovement.speed * moveDuration;
            StartCoroutine(SmoothMove(newposition));
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
            // TODO: fix ease-in, ease-out animation
            // float t = Mathf.SmoothStep(0, 1, t); // Apply ease-in, ease-out
            transform.position = Vector3.Lerp(start, target, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure precise snapping
        isMoving = false;
    }

    Vector3 GetCurrPosition()
    {
        float angle = angleStep * currentLaneIndex * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = Vector3.ProjectOnPlane(newposition, cameraMovement.transform.forward).normalized * tunnelRadius + circleCenter;
        return newposition;
    }

    // public void subtractHealth(float damage){
    //     playerCurrentHealth -= damage;
    //     healthText.text = "Health: " + playerCurrentHealth.ToString();
    //     if (playerCurrentHealth <= 0){
    //         Destroy(gameObject);
    //     }
    // }
}