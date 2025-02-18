using UnityEngine;

public class MyRhythmListener : MonoBehaviour, IEasyListener
{
    public void OnBeat(EasyEvent audioEvent)
    {
        Debug.Log("Beat Detected!");
        // TODO: Add code here to respond to the beat.
    }

}
