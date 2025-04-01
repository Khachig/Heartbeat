using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public GameObject RetryButton;
    public GameObject GameOverText;
    EasyRhythmAudioManager audioManager;
    GameManager gameManager;

    private void Start()
    {
        GameOverText.SetActive(false);
        RetryButton.SetActive(false);    
        Animator anim = gameObject.GetComponent<Animator>();
        anim.SetTrigger("GameOver");
    }

    public void StartShowButton()
    {
        Invoke("ShowButton", 1);
    }

    private void ShowButton()
    {
        GameOverText.SetActive(true);
        RetryButton.SetActive(true);    
        RetryButton.GetComponent<UnityEngine.UI.Button>().Select();
    }

    public void Restart()
    {
        // get the current scene index
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        // reload the current scene
        SceneManager.LoadScene("SpecificArrowLanes");
    }
}
