using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public EnemyManager enemyManager;
    public HealthSystem playerHealth;
    public float missDamage = 10f;

    // Update is called once per frame
    // void Update()
    // {
    //     HandlePlayerInput();
    // }

    void OnAttack(InputValue value)
    {
        // For input handler, only do call back on performed stage
        // if (!value.performed)
            // return;
        Debug.Log("Attacked");
        // bool hitDirection = false;
        // bool hitDirection = true;
        bool successfulHit = false;
        // TODO: Handle if it is on beat
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     hitDirection = true;
        //     successfulHit = enemyManager.handlePlayerAttack(KeyCode.UpArrow);
        // }
        // else if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     hitDirection = true;
        //     successfulHit = enemyManager.handlePlayerAttack(KeyCode.DownArrow);
        // }
        // else if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     hitDirection = true;
        //     successfulHit = enemyManager.handlePlayerAttack(KeyCode.LeftArrow);
        // }
        // else if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     hitDirection = true;
        //     successfulHit = enemyManager.handlePlayerAttack(KeyCode.RightArrow);
        // }
        successfulHit = enemyManager.handlePlayerAttack(value);
        // Miss
        // if (!successfulHit && hitDirection) {
        if (!successfulHit) {
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            playerHealth.TakeDamage(missDamage);
        }
    }
}
