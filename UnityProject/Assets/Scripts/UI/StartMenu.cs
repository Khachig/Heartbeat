using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button playButton, quitButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButton);
        playButton.onClick.AddListener(OnQuitButton);
    }

    /* void Update() */
    /* { */
    /*     if (Input.GetKeyDown(KeyCode.Return)) */
    /*     { */
    /*         Debug.Log($"CLICKED ${SceneManager.GetActiveScene().buildIndex}"); */
    /*         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); */
    /*     } */
    /* } */

    public void OnPlayButton()
    {
        SceneManager.LoadScene("AlphaSceneNewTube");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
