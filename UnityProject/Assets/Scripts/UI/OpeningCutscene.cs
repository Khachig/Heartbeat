using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OpeningCutscene : MonoBehaviour
{
    VideoPlayer video;

    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        video.Play();
        video.loopPointReached += CheckOver;
    }

     void CheckOver(VideoPlayer vp)
    {
        SceneManager.LoadScene("SpecificArrowLanes");
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        bool aPressed = false;
        if (gamepad != null)
        {
            aPressed = gamepad.aButton.wasPressedThisFrame;
        }

        if (Input.GetKeyDown(KeyCode.Return) || aPressed)
        {
            SceneManager.LoadScene("SpecificArrowLanes");
        }
    }
}
