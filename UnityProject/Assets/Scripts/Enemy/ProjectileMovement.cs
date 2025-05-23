using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

public class ProjectileMovement : MonoBehaviour
{
    public float projectileSpeed = 20f;
    public float projectileDamage = 20f;
    public EventReference PlayerHurt;

    private EnemyBehaviour parentEnemy;

    void Update()
    {
        // Slow bullet down because of forward moving player
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            transform.localPosition - Vector3.forward,
            projectileSpeed * Time.deltaTime);

        // If bullet is behind camera, destroy
        if (transform.localPosition.z < -2f)
        {
            if (parentEnemy)
                parentEnemy.onEnemyDestroy -= OnEnemyDestroy;
            Destroy(gameObject);
        }
    }

    public void Init(EnemyBehaviour pBehaviour, float timeToJudgementLine)
    {
        float distanceToJudgementLine = transform.localPosition.z - Stage.Lanes.GetJudgementLineOffset();
        float requiredSpeed = distanceToJudgementLine / timeToJudgementLine;
        projectileSpeed = requiredSpeed;

        SetDestroyCallback(pBehaviour);
    }

    public void SetDestroyCallback(EnemyBehaviour parentBehaviour)
    {
        parentEnemy = parentBehaviour;
        parentEnemy.onEnemyDestroy += OnEnemyDestroy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(projectileDamage);
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            ScoreManager.Instance.DecreaseScore(100);
            RuntimeManager.PlayOneShot(PlayerHurt, transform.position);

            if (parentEnemy)
                parentEnemy.onEnemyDestroy -= OnEnemyDestroy;
            Destroy(gameObject); // Destroy projectile
        }
    }

    private void OnEnemyDestroy()
    {
        if (gameObject)
            Destroy(gameObject);
    }
}
