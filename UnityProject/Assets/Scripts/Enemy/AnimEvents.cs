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
}
