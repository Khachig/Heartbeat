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

    public void Attack()
    {
        EnemyBehaviour eb = transform.parent.gameObject.GetComponent<EnemyBehaviour>();
        eb.Attack();
    }

    public void ArrowAttack()
    {
        EnemyBehaviour eb = transform.parent.gameObject.GetComponent<EnemyBehaviour>();
        eb.ArrowAttack();
    }
}
