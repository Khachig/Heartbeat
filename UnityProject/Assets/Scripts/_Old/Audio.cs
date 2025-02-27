using UnityEngine;
using FMODUnity;

public class MusicPlayer : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string musicEventPath = "event:/TestTrack";

    private FMOD.Studio.EventInstance musicInstance;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEventPath);
        musicInstance.start();
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
}