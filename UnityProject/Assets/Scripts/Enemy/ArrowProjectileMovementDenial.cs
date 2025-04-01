using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class ArrowProjectileMovementDenial : ArrowProjectileMovement 
{
    public float moveDuration = 0.1f;
    private bool hasMoved = false;
    private bool isMoving = false;

    protected override void Update()
    {
        if (!isMoving)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                transform.localPosition - Vector3.forward,
                projectileSpeed * Time.deltaTime);
        }
 
        // If bullet is behind judgement line at threshold, destroy and inflict damage
        if (transform.localPosition.z < (Stage.Lanes.GetJudgementLineOffset() - hitDistanceThreshold))
        {
            HealthSystem playerHealth = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponent<HealthSystem>();
            playerHealth.TakeDamage(projectileDamage);
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            ScoreManager.Instance.DecreaseScore(50);
            RuntimeManager.PlayOneShot(PlayerHurt, transform.position);
            DestroyArrow();
        }
        // If bullet is halfway, switch position to "real" position
        else if (!hasMoved && transform.localPosition.z < (forwardOffset + Stage.Lanes.GetJudgementLineOffset()) * 0.35f)
        {
            hasMoved = true;
            Vector3 pos = GetFinalPositionForArrowDirection(direction);
            StartCoroutine(SmoothMove(pos));
        }
    }

    public override void Init(EnemyBehaviour pBehaviour, float timeToJudgementLine, ArrowDirection direction)
    {
        Vector3 pos = GetInitialPositionForArrowDirection(direction);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = pos;

        float distanceToJudgementLine = transform.localPosition.z - Stage.Lanes.GetJudgementLineOffset();
        float requiredSpeed = distanceToJudgementLine / timeToJudgementLine;
        projectileSpeed = requiredSpeed;

        PlayerAttack playerAttack = GameObject.FindWithTag("Player").GetComponent<PlayerAttack>();
        hitDistanceThreshold = requiredSpeed * playerAttack.hitThreshold;

        SetDestroyCallback(pBehaviour);
    }

    // Returns initial "fake" position for arrow for Denial enemy
    private Vector3 GetInitialPositionForArrowDirection(ArrowDirection direction)
    {
        int lane = 0;
        if (direction == ArrowDirection.DOWN)
            lane = 2;
        else if (direction == ArrowDirection.RIGHT)
            lane = 3;
        else if (direction == ArrowDirection.UP)
            lane = 0;
        else if (direction == ArrowDirection.LEFT)
            lane = 1;
        
        return Stage.Lanes.GetXYPosForLane(lane) + Vector3.forward * forwardOffset;
    }

    // Returns initial "real" position for arrow for Denial enemy
    private Vector3 GetFinalPositionForArrowDirection(ArrowDirection direction)
    {
        int lane = 0;
        if (direction == ArrowDirection.DOWN)
            lane = 0;
        else if (direction == ArrowDirection.RIGHT)
            lane = 1;
        else if (direction == ArrowDirection.UP)
            lane = 2;
        else if (direction == ArrowDirection.LEFT)
            lane = 3;
        
        float finalZ = transform.localPosition.z - moveDuration * projectileSpeed;
        
        return Stage.Lanes.GetXYPosForLane(lane) + new Vector3(0, 0, finalZ);
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 start = transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalize time
            transform.localPosition = Vector3.Lerp(start, target, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = target; // Ensure precise snapping
        isMoving = false;
        yield return null;
    }
}
