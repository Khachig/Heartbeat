using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;
using FMODUnity;
public class PlayerMovement : MonoBehaviour, IEasyListener
{
    public Animator animator;
    public int currentLaneIndex = 3;
    public float moveDuration = 0.3f;
    public float forwardOffset = 10f;
    public float hitThreshold = 0.5f;
    public EventReference PlayerHurt;

    private float timeAtLastBeat;
    private float beatLength;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeAtLastBeat = Time.time;
        transform.localPosition = GetCurrPosition();
        hitThreshold = 0.25f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage

        float currtime = Time.time;
        if (!context.performed)
            return;

        if (isMoving)
            return;

        if (currtime - timeAtLastBeat > hitThreshold &&// lateness threshold
            timeAtLastBeat + beatLength - currtime > hitThreshold) // earliness threshold
        {
            // Do any penalty for moving out of time
            Debug.Log($"{currtime}, {timeAtLastBeat}, {timeAtLastBeat + beatLength} Missed Movement Timing");
            Debug.Log($"{currtime - timeAtLastBeat}, {timeAtLastBeat + beatLength - currtime}");

            return;
        }

        Vector2 moveInput = context.ReadValue<Vector2>();

        if ((IsOnTopLane(currentLaneIndex) && moveInput.x > 0) ||

            (IsOnBottomLane(currentLaneIndex) && moveInput.x < 0) ||

            (IsOnRightLane(currentLaneIndex) && moveInput.y < 0) ||

            (IsOnLeftLane(currentLaneIndex) && moveInput.y > 0) )
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
        else if ((IsOnTopLane(currentLaneIndex) && moveInput.y < 0) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.y > 0) ||
            (IsOnRightLane(currentLaneIndex) && moveInput.x < 0) ||
            (IsOnLeftLane(currentLaneIndex) && moveInput.x > 0))
        {
            StraightLaneChange();
        }
    }

    void StraightLaneChange() {
        Debug.Log($"{currentLaneIndex}, {currentLaneIndex-2}");
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex - 2);
        Debug.Log($"{currentLaneIndex}");
        ChangeLaneStraight();
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
            targetRotation = transform.localRotation * Quaternion.AngleAxis(-angleStep, Vector3.forward);
        else
            targetRotation = transform.localRotation * Quaternion.AngleAxis(angleStep, Vector3.forward);

        StartCoroutine(SmoothMove(newposition, targetRotation));
    }

    void ChangeLaneStraight()
    {
        Vector3 newposition = GetCurrPosition();
        Quaternion targetRotation;

        targetRotation = transform.localRotation * Quaternion.AngleAxis(180, Vector3.forward);
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

        if (Stage.Lanes.IsOffLimitLaneActive(currentLaneIndex))
        {
            HealthSystem playerHealth = transform.GetChild(0).GetComponent<HealthSystem>();
            playerHealth.TakeDamage(5);
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            ScoreManager.Instance.DecreaseScore(25);
            RuntimeManager.PlayOneShot(PlayerHurt, transform.position);
        }
    }
}
