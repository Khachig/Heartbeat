using UnityEngine;

public class PulsableManager : MonoBehaviour
{
    public EasyRhythmAudioManager audioManager;
    public Pulsable[] myPulsables;
    private float bpm;
    private bool initialized = false;
    private bool needReset = false;

    void Update()
    {
        if (bpm == 0)
        {
            bpm = audioManager.myAudioEvent.CurrentTempo / 2f;
            if (bpm > 0 && !initialized)
            {
                // Init all pulsables
                foreach (Pulsable pulsable in myPulsables)
                {
                    pulsable.Init(bpm, audioManager);
                }
                // Workaround since for now easyrhythm takes a while to settle
                // so wait first before syncing beat
                initialized = true;
                Invoke("Reset", 1);
            }
            else if (bpm > 0 && needReset)
            {
                foreach (Pulsable pulsable in myPulsables)
                {
                    pulsable.ResetAnim(bpm);
                }
                needReset = false;
            }
        }
    }

    public void Reset()
    {
        bpm = 0;
        needReset = true;
    }
}
