using UnityEngine;
using System.Collections.Generic;

public class ProjectileMovement : MonoBehaviour
{
    public Stage stage; 
    public float projectileSpeed = 20f;
    public float projectileDamage = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!stage)
            stage = GameObject.Find("Stage").GetComponent<Stage>();
    }

    void Update()
    {
        // Slow bullet down because of forward moving player
        transform.position = Vector3.MoveTowards(
            transform.position,
            transform.position + stage.transform.forward.normalized,
            (stage.speed - projectileSpeed) * Time.deltaTime);

        // If bullet is behind camera, destroy
        if (transform.position.z - stage.transform.position.z < -2f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(projectileDamage);
			Effects.SpecialEffects.ScreenDamageEffect(0.5f);
            Destroy(gameObject); // Destroy projectile
        }
    }
}
