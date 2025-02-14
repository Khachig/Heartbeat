using UnityEngine;
using System.Collections;
using TMPro;
public class PlayerMovement : MonoBehaviour
{
    public Stage stage;
    public int currentLaneIndex = 0;
    public float moveDuration = 0.3f;
    private bool isMoving = false;
    
    public Vector3 circleCenter;
    public float forwardOffset = 10f; 

    public float playerMaxHealth = 100f;
    public float playerCurrentHealth = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!stage)
            stage = GameObject.Find("Stage").GetComponent<Stage>();

        UpdateCircleCenter();
        MoveWithStage();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCircleCenter();
        MoveWithStage();
        if (!isMoving){
            HandleChangeLaneInupt();
        }
        
    }

    void UpdateCircleCenter()
    {
        // Set the center of the movement circle slightly in front of the camera
        circleCenter = stage.transform.position + stage.transform.forward * forwardOffset;
    }

    void MoveWithStage()
    {
        // Ensure the player stays at the correct position even when the camera moves
        transform.position = GetCurrPosition();
    }

    void HandleChangeLaneInupt()
    {
        if (Input.GetKeyDown(KeyCode.D))
        { 
            currentLaneIndex = (currentLaneIndex + 1) % stage.numLanes;
            ChangeLane();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentLaneIndex = (currentLaneIndex - 1) % stage.numLanes;
            if (currentLaneIndex < 0)
                currentLaneIndex += stage.numLanes;
            ChangeLane();
        }
        
    }

    void ChangeLane()
    {
        Vector3 newposition = GetCurrPosition() + stage.transform.forward * stage.speed * moveDuration;
        StartCoroutine(SmoothMove(newposition));
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
        float angleStep = 360f / stage.numLanes;
        float angle = angleStep * currentLaneIndex * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = Vector3.ProjectOnPlane(newposition, stage.transform.forward).normalized * stage.tunnelRadius + circleCenter;
        return newposition;
    }
}