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
    public float phaseTransitionDuration = 0.3f;
    public float forwardOffset = 10f;
    private float hitThreshold = 0.20f;
    public EventReference PlayerHurt;

    private float timeAtLastBeat;
    private float beatLength;
    private bool isMoving = false;
    private float lastMoveTime = 0f;
    private const float inputCooldown = 0.1f; // set to 0.1 second
    private bool canMove = true;

    private int beatMultiplier = 1;
    private int beatMultiplierIfHit = 2;
    private float timeAtLastOffBeat;
    private int globalBeatOffLimitLaneSpawn = 0;
    private bool inLevelEndScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeAtLastBeat = Time.time;
        transform.localPosition = GetCurrPosition();
        hitThreshold = 0.2f;
    }
    public void SetBeatOffLimitLaneSpawn(int beat){
        globalBeatOffLimitLaneSpawn = beat;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove)
            return;

        // For input handler, only do call back on performed stage

        float currtime = Time.time;
        if (!context.performed)
            return;

        if (isMoving)
            return;

        if (currtime - lastMoveTime < inputCooldown)
            return;

        if (currtime - timeAtLastBeat > hitThreshold && // lateness threshold
            timeAtLastBeat + beatLength - currtime > hitThreshold) // earliness threshold
        {
            // Do any penalty for moving out of time
            Debug.Log($"{currtime}, {timeAtLastBeat}, {timeAtLastBeat + beatLength} Missed Movement Timing");
            Debug.Log($"{currtime - timeAtLastBeat}, {timeAtLastBeat + beatLength - currtime}");

            return;
        }


        Vector2 moveInput = context.ReadValue<Vector2>();

        //Debug.Log($"Move Input: {moveInput}");

        //normalize the input
        moveInput.x = Mathf.Round(moveInput.x * 2) / 2;
        moveInput.y = Mathf.Round(moveInput.y * 2) / 2;

        // if the input is from joystick, we need to normalize it

        if ((IsOnTopLane(currentLaneIndex) && moveInput.x > 0 && (moveInput.y < 0.5 && moveInput.y > -0.5)) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.x < 0 && (moveInput.y < 0.5 && moveInput.y > -0.5)) ||
            (IsOnRightLane(currentLaneIndex) && moveInput.y < 0 && (moveInput.x < 0.5 && moveInput.x > -0.5)) ||
            (IsOnLeftLane(currentLaneIndex) && moveInput.y > 0 && (moveInput.x < 0.5 && moveInput.x > -0.5)))
        {
            ClockwiseLaneChange();
        }
        else if ((IsOnTopLane(currentLaneIndex) && moveInput.x < 0 && (moveInput.y < 0.5 && moveInput.y > -0.5)) ||
            (IsOnBottomLane(currentLaneIndex) && moveInput.x > 0 && (moveInput.y < 0.5 && moveInput.y > -0.5)) ||
            (IsOnRightLane(currentLaneIndex) && moveInput.y > 0 && (moveInput.x < 0.5 && moveInput.x > -0.5)) ||
            (IsOnLeftLane(currentLaneIndex) && moveInput.y < 0 && (moveInput.x < 0.5 && moveInput.x > -0.5)))
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

        lastMoveTime = currtime;
    }

    void StraightLaneChange()
    {
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex - 2);
        MoveToCurrLane(moveDuration);
    }

    void ClockwiseLaneChange()
    {
        animator.SetTrigger("MoveLeft");
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex - 1);
        MoveToCurrLane(moveDuration);
    }

    void CounterClockwiseLaneChange()
    {
        animator.SetTrigger("MoveRight");
        currentLaneIndex = Stage.Lanes.GetModLane(currentLaneIndex + 1);
        MoveToCurrLane(moveDuration);
    }
    
    void MoveToCurrLane(float transitionDuration)
    {
        Vector3 newposition = GetCurrPosition();
        Quaternion targetRotation = GetCurrRotation();
        if (!inLevelEndScreen){
            IncrementMultiplier(1);
        }
        StartCoroutine(SmoothMove(newposition, targetRotation, transitionDuration));
    }

    IEnumerator SmoothMove(Vector3 target, Quaternion targetRotation, float transitionDuration)
    {
        isMoving = true;
        Vector3 start = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        float elapsedTime = 0;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration; // Normalize time
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

    Quaternion GetCurrRotation()
    {
        float angleStep = 360f / Stage.Lanes.GetNumLanes();

        return Quaternion.AngleAxis(angleStep * currentLaneIndex, Vector3.forward);
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
        if (timeAtLastOffBeat == 0){
            timeAtLastOffBeat = timeAtLastBeat;
        }

        // beatMultiplierIfHit++;
        if (timeAtLastBeat >= timeAtLastOffBeat + beatLength + 0.2){
            beatMultiplierIfHit++;
            timeAtLastOffBeat = timeAtLastBeat;
        }
        
        // Debug.Log($"beatMIfHit {beatMultiplierIfHit} {timeAtLastOffBeat} {timeAtLastBeat}");
        // Start coroutine to check for missed hit 0.21s later
        StartCoroutine(CheckMultiplierMissAfterDelay(0.21f));

        if (Stage.Lanes.IsOffLimitLaneActive(currentLaneIndex) && audioEvent.CurrentBar*8 + audioEvent.CurrentBeat > globalBeatOffLimitLaneSpawn + 4)
        {
            HealthSystem playerHealth = transform.GetChild(0).GetComponent<HealthSystem>();
            playerHealth.TakeDamage(5);
            Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            ScoreManager.Instance.DecreaseScore(25);
            RuntimeManager.PlayOneShot(PlayerHurt, transform.position);
        }
    }

    public void SetInLevelEndScreen(bool inScreen){
        inLevelEndScreen = inScreen;
    }

    private IEnumerator CheckMultiplierMissAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (inLevelEndScreen){
            beatMultiplierIfHit = beatMultiplier;
        }
        else if (beatMultiplierIfHit > beatMultiplier)
        {
            BreakMultiplier();
            beatMultiplierIfHit = beatMultiplier;
        }
    
    }
    public void IncrementMultiplier(int amount)
    {
        beatMultiplier += amount;
        // if (beatMultiplier > beatMultiplierIfHit + 1){
        //     beatMultiplierIfHit = beatMultiplier;
        // }
        ScoreManager.Instance.SetMultiplier(beatMultiplier);
        // Debug.Log($"✅ Combo hit! Multiplier: {beatMultiplier}");
        Effects.SpecialEffects.MultiplierEffect(beatMultiplier);
    }
    private void BreakMultiplier()
    {
        beatMultiplier = Mathf.Max(1, beatMultiplier - 3);
        ScoreManager.Instance.SetMultiplier(beatMultiplier);
    }
}
