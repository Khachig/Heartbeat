using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public delegate void OnAttackMiss();
    public static OnAttackMiss onAttackMiss;

    public EnemyManager enemyManager;
    public HealthSystem playerHealth;
    public float missDamage = 10f;
    public float hitThreshold = 0.1f;
    public EasyRhythmAudioManager audioManager;
    private float timeAtLastBeat;
    private float beatLength;

    void Start()
    {
        if (!audioManager)
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();

        beatLength = audioManager.myAudioEvent.BeatLength();
        timeAtLastBeat = Time.time;
        BeatCapturer.onBeatCapture += OnBeatCaptured;
    }

    void Update()
    {
        if (beatLength == 0)
        {
            beatLength = audioManager.myAudioEvent.BeatLength();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;
        
        // Missed timing
        if (Time.time - timeAtLastBeat > hitThreshold && // lateness threshold
            timeAtLastBeat + beatLength - Time.time > hitThreshold) // earliness threshold
        {
            AttackMiss();
            return;
        }

        bool successfulHit = enemyManager.handlePlayerAttack(context.ReadValue<Vector2>());
        if (!successfulHit)
            AttackMiss();
    }

    void AttackMiss()
    {
        Effects.SpecialEffects.ScreenDamageEffect(0.5f);
        playerHealth.TakeDamage(missDamage);
        onAttackMiss?.Invoke();
    }

    void OnBeatCaptured()
    {
        timeAtLastBeat = Time.time;
    }
}
