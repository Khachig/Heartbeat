using UnityEngine;

public class PulsableManager : MonoBehaviour
{
    public EasyRhythmAudioManager audioManager;
    public Pulsable[] myPulsables;
    private float bpm;

    void Update()
    {
        if (bpm == 0)
        {
            bpm = audioManager.myAudioEvent.CurrentTempo;
            if (bpm > 0)
            {
                // Init all pulsables
                foreach (Pulsable pulsable in myPulsables)
                {
                    pulsable.Init(bpm, audioManager);
                    Debug.Log("initing pusable w bpm! " + bpm);
                }
            }
        }
    }
}
