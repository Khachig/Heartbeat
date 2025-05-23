using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IEasyListener
{
    public delegate void OnAttackMiss();
    public static OnAttackMiss onAttackMiss;

    public EnemyManager enemyManager;
    public EnemyRhythmManager enemyRhythmManager;
    public HealthSystem playerHealth;
    public float missDamage = 5f;
    public float hitThreshold = 0.1f;
    private float timeAtLastBeat;
    private float beatLength;

    void Start()
    {
        timeAtLastBeat = Time.time;
        hitThreshold = 0.2f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;

        // Only attack when game is unpaused
        if (Time.timeScale == 0)
            return;
        
        // Missed timing
        if (Time.time - timeAtLastBeat > hitThreshold && // lateness threshold
            timeAtLastBeat + beatLength - Time.time > hitThreshold) // earliness threshold
        {
            AttackMiss();
            return;
        }

        // bool successfulHit = enemyManager.handlePlayerAttack(context.ReadValue<Vector2>());
        bool successfulHit = enemyRhythmManager.HandlePlayerAttack(context.ReadValue<Vector2>());
        if (!successfulHit)
            AttackMiss();
    }

    void AttackMiss()
    {
        ScoreManager.Instance.DecreaseScore(25);
        Effects.SpecialEffects.MissEffect();
        Effects.SpecialEffects.ScreenDamageEffect(0.5f);
        playerHealth.TakeDamage(missDamage);
        onAttackMiss?.Invoke();
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (beatLength == 0)
            beatLength = audioEvent.BeatLength();
        timeAtLastBeat = Time.time;
    }
}
