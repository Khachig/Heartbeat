using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    ScoreManager scoreManager;
    EasyRhythmAudioManager audioManager;

    public void Start()
    {
        audioManager = Object.FindAnyObjectByType<EasyRhythmAudioManager>();
        scoreManager = Object.FindAnyObjectByType<ScoreManager>();
    }

    public void Restart()
    {
        audioManager.Reset();
        // get the current scene index
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        // reload the current scene
        SceneManager.LoadScene(currentSceneIndex);

        scoreManager.ReInitScore();
    }


}
