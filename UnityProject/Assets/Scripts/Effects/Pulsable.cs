using UnityEngine;

// Component to be used by any in game object that needs to pulse to the beat
// All users must have an Animator with float "PulseSpeed" and trigger "Reset"
public class Pulsable : MonoBehaviour, IEasyListener
{
    
    public Animator anim;
    private float bpm;

    public void Init(float newBpm, EasyRhythmAudioManager audioManager)
    {
        audioManager.AddListener(this);
        bpm = newBpm;
        anim.SetFloat("PulseSpeed", bpm / 60f);
    }

    public void ResetAnim(float newBpm = 0f)
    {
        if (newBpm == 0)
            newBpm = bpm;
        else
            bpm = newBpm;
        anim.SetFloat("PulseSpeed", bpm / 60f);
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (anim == null)
            return;

        if (audioEvent.CurrentBeat % 2 != 0)
        {
            anim.SetTrigger("Reset");
        }
    }
}