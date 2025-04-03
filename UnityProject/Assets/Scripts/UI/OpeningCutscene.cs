using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

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
}
