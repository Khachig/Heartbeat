using UnityEngine;

public class OldEnemyMovement : MonoBehaviour
{
    public float hover_distance = 0.005f;
    public float hover_speed = 0.05f;

    void Update()
    {
        float y_offset = hover_distance * Mathf.Sin(Time.frameCount * hover_speed);
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + y_offset,
            transform.position.z
        );
    }
}

/* public class EnemyMovement : MonoBehaviour */
/* { */
/*     private const float lerp_duration = .4f; */
/*     private Vector3 start_position; */
/*     private Vector3 target_position; */
/*     private float lerp_start_time; */
/*     [SerializeField] private float movement_speed = 3.5f; */

/*     private void Start() */
/*     { */
/*         start_position = transform.position; */
/*         target_position = new Vector3( */
/*             start_position.x, */
/*             start_position.y, */
/*             start_position.z */
/*         ); */
/*         start_position = target_position; */
/*         transform.position = target_position; */
/*         lerp_start_time = Time.time; */
/*     } */

/*     private void Update() */
/*     { */
/*         float t = (Time.time - lerp_start_time) / lerp_duration; */
/*         transform.position = Vector3.Lerp(start_position, target_position, t); */
/*         if (transform.position == target_position) */
/*         { */
/*             lerp_start_time = 0f; */
/*             target_position.z -= movement_speed / 100f; */
/*         } */
/*         if (transform.position.z <= -15) */
/*         { */
/*             Destroy(gameObject); */
/*             Destroy(this); */
/*         } */
/*     } */
/* } */

