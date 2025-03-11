using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class ArrowProjectileMovement : MonoBehaviour
{
    public delegate void OnArrowDestroy();
    // To be invoked whenever an arrow is destroyed
    public OnArrowDestroy onArrowDestroy;
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;
    public EventReference PlayerHurt;

    private EnemyBehaviour parentEnemy;
    private float hitDistanceThreshold = 0f;

    void Update()
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

    public void Init(EnemyBehaviour pBehaviour, float timeToJudgementLine)
    {
        transform.localRotation = Quaternion.identity;

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
        return Mathf.Abs(Stage.Lanes.GetJudgementLineOffset() - transform.localPosition.z) < hitDistanceThreshold;
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

    private void OnEnemyDestroy()
    {
        if (gameObject)
            Destroy(gameObject);
    }
}
