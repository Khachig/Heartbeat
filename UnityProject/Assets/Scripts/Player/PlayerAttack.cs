using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour, IEasyListener
{
    public delegate void OnAttackMiss();
    public static OnAttackMiss onAttackMiss;
    public delegate void OnAttackSuccess();
    public static OnAttackSuccess onAttackSuccess;

    public EnemyManager enemyManager;
    public HealthSystem playerHealth;
    public float missDamage = 10f;
    public float hitThreshold = 0.1f;
    private float timeAtLastBeat;
    private float beatLength;

    void Start()
    {
        timeAtLastBeat = Time.time;
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

        bool successfulHit = enemyManager.handlePlayerAttack(context.ReadValue<Vector2>());
        if (successfulHit)
            onAttackSuccess?.Invoke();
        else
            AttackMiss();
    }

    void AttackMiss()
    {
        ScoreManager.Instance.DecreaseScore(30);
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
