using UnityEngine;
using System.Collections.Generic;
public class ProjectileMovement : MonoBehaviour
{
    public Camera mainCamera;
    public int enemyLane;
    private Vector3[] wayPoints;
	private int currentWaypointIndex = 0;

    // public GameObject projectilePrefab;
    
    // public Transform[] pathWaypoints;
    public float projectileSpeed = 20f;
    public int numberOfWaypoints = 10;
    public float tunnelRadius = 10;
    
    public float forwardOffset = 10f; 
    public float enemyDistance = 50f;
    public int projectileDamage = 10;

    private Vector3 spawnPos;
    private Vector3 targetPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalculatePositions();
        // spawnPos = enemyPlaneLocation.position + (enemyPlaneLocation.forward * projectileDistance);;
        transform.position = spawnPos;
        SetWaypoints();
        
    }

    void CalculatePositions(int numLanes = 4)
    {
        Vector3 targetCenter = mainCamera.transform.position + mainCamera.transform.forward * forwardOffset;

        Vector3 circleCenter = new Vector3(targetCenter[0], targetCenter[1], targetCenter[2]+enemyDistance);
        // Vector3 targetCenter = playerPlaneLocation.position;
        // float tunnelRadius = enemyPlaneLocation.localScale.x/2;
        Debug.Log("tunnelRadius " + tunnelRadius);
        float angleStep = 360f / numLanes;

        float angle = angleStep * enemyLane * Mathf.Deg2Rad;
        spawnPos = circleCenter + new Vector3(Mathf.Cos(angle) * tunnelRadius, Mathf.Sin(angle) * tunnelRadius, 0);
        
        targetPos = targetCenter + new Vector3(Mathf.Cos(angle) * tunnelRadius, Mathf.Sin(angle) * tunnelRadius, 0);
        
    }

    // enemies are offset from the player 
    // projectiles start with offset but offset decreases?
    // All move forward with camera

    // let offset be difference in index between player waypoints and enemy

    // void Update()
    // {
    //     if (Time.time >= nextFireTime)
    //     {
    //         SpawnProjectile();
    //         nextFireTime = Time.time + 1f / fireRate;
    //     }
    // }
    // public void SetPoints(List<Vector3> newPoints) {
    //     this.points = newPoints;
    //     SetTargetPoint();
    // }

    // void SpawnProjectile()
    // {
    //     GameObject projectile = Instantiate(projectilePrefab, enemyLocation.position, Quaternion.identity);
    //     Projectile projScript = projectile.GetComponent<Projectile>();
    //     projScript.waypoints = pathWaypoints; // Assign waypoints
    // }

    public void SetWaypoints() // List<Vector3> pathPoints
    {
        wayPoints = new Vector3[numberOfWaypoints];

        Vector3 direction = (targetPos - spawnPos).normalized;

        Debug.Log("direction" +direction.ToString());

        float totalDistance = Vector3.Distance(spawnPos, targetPos);

        // Compute equal spacing between waypoints
        float waypointDistance = totalDistance / numberOfWaypoints;

        // Generate waypoints in a straight line
        for (int i = 1; i < numberOfWaypoints ; i++)
        {
            Vector3 waypoint = spawnPos + direction * (i * waypointDistance);
            // Debug.Log("waypoint " + i + waypoint.ToString());
            wayPoints[i-1] = waypoint;
        }
        Vector3 furtherWaypoint = spawnPos + direction * (numberOfWaypoints * waypointDistance);
        // Debug.Log("furtherWaypoint " + 10 + furtherWaypoint.ToString());
        wayPoints[numberOfWaypoints - 1] = furtherWaypoint;
    }

    void Update()
    {
        if (wayPoints == null) return;

        // Move toward the current waypoint
        Vector3 targetPosition = wayPoints[currentWaypointIndex];
        // Vector3 finalPosition = wayPoints[numberOfWaypoints - 1];

        // Debug.Log("waypoint final " + targetPosition.ToString());
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, projectileSpeed * Time.deltaTime);

        // Debug.Log("Player: "+ player.position);

        // Check if the projectile reached the waypoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            currentWaypointIndex++;

            // If all wayPoints are reached, stop or destroy
            if (currentWaypointIndex >= numberOfWaypoints)
            {
                Destroy(gameObject); // Or trigger hit effect
                // transform.position = spawnPos;
                // currentWaypointIndex = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Ensure the player has tag "Player"
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            playerHealth.TakeDamage(projectileDamage);
            Destroy(gameObject); // Destroy projectile
        }
    }
}
