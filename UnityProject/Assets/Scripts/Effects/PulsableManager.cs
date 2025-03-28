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
            bpm = audioManager.myAudioEvent.CurrentTempo / 2f;
            if (bpm > 0)
            {
                // Init all pulsables
                foreach (Pulsable pulsable in myPulsables)
                {
                    pulsable.Init(bpm, audioManager);
                }
                // Workaround since for now easyrhythm takes a while to settle
                // so wait first before syncing beat
                Invoke("Reset", 1);
            }
        }
    }

    public void Reset()
    {
        foreach (Pulsable pulsable in myPulsables)
        {
            pulsable.ResetAnim();
        }
    }
}
