using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }

    public void QueueResetEnemyAnim()
    {
        transform.parent.GetComponent<EnemyBehaviour>().QueueResetEnemyAnim();
    }
}
