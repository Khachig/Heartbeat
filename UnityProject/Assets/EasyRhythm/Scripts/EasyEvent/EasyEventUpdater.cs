using UnityEngine;

/// <summary>
/// This is a singleton that runs the Update() method in all instances of EasyEvent.
/// </summary>
public class EasyEventUpdater : MonoBehaviour
{
    public delegate void UpdateEvent();
    public static event UpdateEvent OnUpdate; // called on every beats

    public static EasyEventUpdater instance = null;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        OnUpdate();
    }
}
