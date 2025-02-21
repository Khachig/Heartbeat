using UnityEngine;

public class BeatCapturer : MonoBehaviour, IEasyListener
{
    public delegate void OnBeatCapture();
    public static OnBeatCapture onBeatCapture;
    public void OnBeat(EasyEvent audioEvent)
    {
        // Debug.Log("Beat captured!");
        onBeatCapture?.Invoke();
    }

}
