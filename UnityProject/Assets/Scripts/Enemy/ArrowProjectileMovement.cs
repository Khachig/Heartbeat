using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class ArrowProjectileMovement : MonoBehaviour
{
    public delegate void OnArrowDestroy();
    // To be invoked whenever an arrow is destroyed
    public OnArrowDestroy onArrowDestroy;
    public ArrowDirection direction;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;
    public float forwardOffset = 80f;
    public EventReference PlayerHurt;

    protected EnemyBehaviour parentEnemy;
    protected float hitDistanceThreshold = 0f;

    protected virtual void Update()
    {
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            transform.localPosition - Vector3.forward,
            projectileSpeed * Time.deltaTime);

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
    }

    public ArrowDirection GetArrowDirection()
    {
        return direction;
    }

    public virtual void Init(EnemyBehaviour pBehaviour, float timeToJudgementLine, ArrowDirection direction)
    {
        Vector3 pos = GetPositionForArrowDirection(direction);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = pos;

        float distanceToJudgementLine = transform.localPosition.z - Stage.Lanes.GetJudgementLineOffset();
        float requiredSpeed = distanceToJudgementLine / timeToJudgementLine;
        projectileSpeed = requiredSpeed;

        PlayerAttack playerAttack = GameObject.FindWithTag("Player").GetComponent<PlayerAttack>();
        hitDistanceThreshold = requiredSpeed * playerAttack.hitThreshold;

        SetDestroyCallback(pBehaviour);
    }

    public void SetDestroyCallback(EnemyBehaviour parentBehaviour)
    {
        parentEnemy = parentBehaviour;
        parentEnemy.onEnemyDestroy += OnEnemyDestroy;
    }

    public bool IsInHitRange()
    {
        return SameLaneAsPlayer() && Mathf.Abs(Stage.Lanes.GetJudgementLineOffset() - transform.localPosition.z) < hitDistanceThreshold;
    }

    public bool IsInContactOfPlayer()
    {
        Vector3 distance = GameObject.FindWithTag("Player").transform.position - transform.position;
        float playerHitDistanceThreshold = 3.0f;
        bool withinZ = Mathf.Abs(distance.z) <= hitDistanceThreshold;
        bool withinX = Mathf.Abs(distance.x) <= 5.5f;
        bool withinY = Mathf.Abs(distance.y) <= 2f;

        if (withinZ && withinX && withinY)
        {
            Debug.Log($"Player in offset distance {distance}.");
            return true;
        }
        Debug.Log($"not offset distance {playerHitDistanceThreshold} {distance}.");
        return false;
    }

    public bool SameLaneAsPlayer()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
        if (playerMovement.currentLaneIndex == GetLaneForArrowDirection(direction)){
            return true;
        } 
        return false;
    }

    public void DestroyArrow()
    {
        if (parentEnemy)
            parentEnemy.onEnemyDestroy -= OnEnemyDestroy;
        onArrowDestroy?.Invoke();
        Destroy(gameObject); // Destroy projectile
    }

    public EnemyBehaviour GetParentEnemyBehaviour()
    {
        return parentEnemy;
    } 

    protected void OnEnemyDestroy()
    {
        if (gameObject)
            Destroy(gameObject);
    } 

    protected Vector3 GetPositionForArrowDirection(ArrowDirection direction)
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
        
        return Stage.Lanes.GetXYPosForLane(lane) + Vector3.forward * forwardOffset;
    }

    protected int GetLaneForArrowDirection(ArrowDirection direction)
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
        
        return lane;
    }
}
