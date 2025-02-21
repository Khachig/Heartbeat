using UnityEngine;

public class BeatCapturer : MonoBehaviour, IEasyListener
{
    public delegate void OnBeatCapture();
    public static OnBeatCapture onBeatCapture;
    public void OnBeat(EasyEvent audioEvent)
    {
        onBeatCapture?.Invoke();
    }

}
