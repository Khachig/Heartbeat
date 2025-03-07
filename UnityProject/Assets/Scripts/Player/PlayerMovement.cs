using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour, IEasyListener
{
    public Animator animator;
    public int currentLaneIndex = 3;
    public float moveDuration = 0.3f;
    public float forwardOffset = 10f;
    public float hitThreshold = 0.2f;

    private float timeAtLastBeat;
    private float beatLength;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeAtLastBeat = Time.time;
        transform.localPosition = GetCurrPosition();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;

        if (isMoving)
            return;

        if (Time.time - timeAtLastBeat > hitThreshold && // lateness threshold
            timeAtLastBeat + beatLength - Time.time > hitThreshold) // earliness threshold
        {
            // Do any penalty for moving out of time
            Debug.Log("Missed Movement Timing");
            return;
        }

        Vector2 moveInput = context.ReadValue<Vector2>();

        if ((IsOnTopLane(currentLaneIndex) && moveInput.x > 0) ||
            (IsOnTopLane(currentLaneIndex) && moveInput.y < 0) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.x < 0) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.y > 0) ||

            (IsOnRightLane(currentLaneIndex) && moveInput.y < 0) ||
            (IsOnRightLane(currentLaneIndex) && moveInput.x < 0) ||

            (IsOnLeftLane(currentLaneIndex) && moveInput.y > 0) ||
            (IsOnLeftLane(currentLaneIndex) && moveInput.x > 0))
        {
            ClockwiseLaneChange();
        }
        else if ((IsOnTopLane(currentLaneIndex) && moveInput.x < 0) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.x > 0) ||
            (IsOnRightLane(currentLaneIndex) && moveInput.y > 0) ||
            (IsOnLeftLane(currentLaneIndex) && moveInput.y < 0))
        {
            CounterClockwiseLaneChange();
        }
    }

    void ClockwiseLaneChange() {
        animator.SetTrigger("MoveLeft");
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex - 1);
        ChangeLane(true);
    }

    void CounterClockwiseLaneChange() {
        animator.SetTrigger("MoveRight");
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex + 1);
        ChangeLane(false);
    }

    void ChangeLane(bool moveLeft)
    {
        Vector3 newposition = GetCurrPosition();
        Quaternion targetRotation;
        float angleStep = 360f / Stage.Lanes.GetNumLanes();

        if (moveLeft)
            targetRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z - angleStep);
        else
            targetRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angleStep);

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
        Vector3 newposition = Stage.Lanes.GetXYPosForLane(currentLaneIndex) + Vector3.forward * forwardOffset;
        return newposition;
    }

    bool IsOnTopLane(int lane) { return lane == (Stage.Lanes.GetNumLanes() / 2); }

    bool IsOnBottomLane(int lane) { return lane == 0; } 
    
    bool IsOnLeftLane(int lane)
    {
        return (Stage.Lanes.GetNumLanes() / 2) < lane &&
               lane < Stage.Lanes.GetNumLanes();
    } 

    bool IsOnRightLane(int lane)
    {
        return 0 < lane &&
               lane < (Stage.Lanes.GetNumLanes() / 2);
    } 

    public void OnBeat(EasyEvent audioEvent)
    {
        if (beatLength == 0)
            beatLength = audioEvent.BeatLength();
        timeAtLastBeat = Time.time;
    }
}
