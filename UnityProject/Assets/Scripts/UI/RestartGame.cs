using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    EasyRhythmAudioManager audioManager;
    GameManager gameManager;

    public void Start()
    {
        audioManager = Object.FindAnyObjectByType<EasyRhythmAudioManager>();
        gameManager = Object.FindAnyObjectByType<GameManager>();
    }

    public void Restart()
    {
        audioManager.Reset();
        // get the current scene index
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        // reload the current scene
        SceneManager.LoadScene(currentSceneIndex);

        gameManager.Restart();
    }


}
