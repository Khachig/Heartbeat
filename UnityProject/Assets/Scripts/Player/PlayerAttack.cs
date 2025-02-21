using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public EnemyManager enemyManager;
    public HealthSystem playerHealth;
    public float missDamage = 10f;

    public void OnAttack(InputAction.CallbackContext context)
    {
        // For input handler, only do call back on performed stage
        if (!context.performed)
            return;

        bool successfulHit = enemyManager.handlePlayerAttack(context.ReadValue<Vector2>());
        if (!successfulHit) {
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            playerHealth.TakeDamage(missDamage);
        }
    }
}
