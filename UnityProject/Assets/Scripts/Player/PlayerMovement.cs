using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Stage stage;
    public int currentLaneIndex = 3;
    public float moveDuration = 0.3f;
    private bool isMoving = false;

    public Vector3 circleCenter;
    public float forwardOffset = 10f;

    public float playerMaxHealth = 100f;
    public float playerCurrentHealth = 100f;
    
    public Animator animator;

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

    public void OnMove(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;

        if (isMoving)
            return;

        // Vector2 moveInput = value.Get<Vector2>();
        Vector2 moveInput = context.ReadValue<Vector2>();

        // Move right
        if (moveInput.x > 0)
        {
            animator.SetTrigger("MoveRight");
            currentLaneIndex = (currentLaneIndex + 1) % stage.numLanes;
            ChangeLane(false);
        }
        // Move left
        else if (moveInput.x < 0)
        {
            animator.SetTrigger("MoveLeft");
            currentLaneIndex = (currentLaneIndex - 1) % stage.numLanes;
            if (currentLaneIndex < 0)
                currentLaneIndex += stage.numLanes;
            ChangeLane(true);
        }

    }

    void ChangeLane(bool moveLeft)
    {
        Vector3 newposition = GetCurrPosition() + stage.transform.forward * stage.speed * moveDuration;
        Quaternion targetRotation;
        if (moveLeft)
            targetRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z - 90);
        else
            targetRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + 90);

        StartCoroutine(SmoothMove(newposition, targetRotation));
    }

    IEnumerator SmoothMove(Vector3 target, Quaternion targetRotation)
    {
        isMoving = true;
        Vector3 start = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalize time
            transform.position = Vector3.Lerp(start, target, t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure precise snapping
        transform.rotation = targetRotation;
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
