using UnityEngine;

public class LevelCompleteScreen : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.GetComponentInChildren<UnityEngine.UI.Button>().Select();
    }

    public void DisableScreen()
    {
        gameObject.SetActive(false);
    } 
}
