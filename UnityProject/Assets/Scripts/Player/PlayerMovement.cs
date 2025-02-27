using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public Stage stage;
    public int currentLaneIndex = 3;
    public float moveDuration = 0.3f;
    public float forwardOffset = 10f;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!stage)
            stage = GameObject.Find("Stage").GetComponent<Stage>();

        transform.localPosition = GetCurrPosition();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;

        if (isMoving)
            return;

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
        // Vector3 newposition = GetCurrPosition() + stage.transform.forward * stage.speed * moveDuration;
        Vector3 newposition = GetCurrPosition();
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
        Vector3 start = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalize time
            transform.localPosition = Vector3.Lerp(start, target, t);
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target; // Ensure precise snapping
        transform.localRotation = targetRotation;
        isMoving = false;
    }

    Vector3 GetCurrPosition()
    {
        float angleStep = 360f / stage.numLanes;
        float angle = angleStep * currentLaneIndex * Mathf.Deg2Rad;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = newposition.normalized * stage.tunnelRadius + Vector3.forward * forwardOffset;
        return newposition;
    }
}
