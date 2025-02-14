using UnityEngine;
using System.Collections.Generic;
public class ProjectileMovement : MonoBehaviour
{
    public Camera mainCamera;
    private CameraMovement cameraMovement;
    public int enemyLane = 0;

    public float projectileDistance;
    private Vector3[] wayPoints;
	private int currentWaypointIndex = 0;

    // public GameObject projectilePrefab;
    
    // public Transform[] pathWaypoints;
    public float projectileSpeed = 20f;
    public int numberOfWaypoints = 10;
    public float tunnelRadius = 10;
    
    public float forwardOffset = 10f; 
    public float enemyDistance = 6f;
    public float projectileDamage = 10f;

    public Vector3 spawnPos;
    private Vector3 targetPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!mainCamera){
            mainCamera = Camera.main;
        }
        cameraMovement = mainCamera.GetComponent<CameraMovement>();
        tunnelRadius = 3f;
        // CalculatePositions();
        // spawnPos = enemyPlaneLocation.position + (enemyPlaneLocation.forward * projectileDistance);;
        // transform.position = spawnPos;
        // SetWaypoints();
        
    }

    void CalculatePositions(int numLanes = 4)
    {
        Vector3 targetCenter = mainCamera.transform.position + mainCamera.transform.forward * 0;

        Vector3 circleCenter = new Vector3(targetCenter.x, targetCenter.y, targetCenter.z + enemyDistance);
        // Vector3 targetCenter = playerPlaneLocation.position;
        // float tunnelRadius = enemyPlaneLocation.localScale.x/2;
        float angleStep = 360f / numLanes;

        float angle = angleStep * enemyLane * Mathf.Deg2Rad;
        spawnPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        spawnPos = Vector3.ProjectOnPlane(spawnPos, mainCamera.transform.forward).normalized * tunnelRadius + circleCenter;       

        targetPos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        targetPos = Vector3.ProjectOnPlane(targetPos, mainCamera.transform.forward).normalized * tunnelRadius + targetCenter;       
        
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
        if (cameraMovement) {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + mainCamera.transform.forward.normalized, (cameraMovement.speed / 2f) * Time.deltaTime);
        }
        if (wayPoints == null) return;

        // Move toward the current waypoint
        Vector3 targetPosition = wayPoints[currentWaypointIndex];
        // Vector3 finalPosition = wayPoints[numberOfWaypoints - 1];

        // Debug.Log("waypoint final " + targetPosition.ToString());
        
        // transform.position = Vector3.MoveTowards(transform.position, targetPosition, projectileSpeed * Time.deltaTime); 

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
