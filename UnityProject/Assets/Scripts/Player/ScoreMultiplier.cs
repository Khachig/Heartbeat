using UnityEngine;
using TMPro;
using System.Collections;
using FMODUnity;

public class ScoreMultiplier : MonoBehaviour, IEasyListener
{
    public static ScoreMultiplier Instance { get; private set; }

    public EasyRhythmAudioManager audioManager;

    private int beatMultiplier = 1;
    private int beatMultiplierIfHit = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!audioManager)
        {
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();
            
        }
        Debug.Log(audioManager);
        audioManager.AddListener(this);
        
    }

    private IEnumerator CheckMultiplierMissAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (beatMultiplierIfHit > beatMultiplier)
        {
            BreakMultiplier();
            beatMultiplierIfHit = beatMultiplier;
        }
    
    }
    private void IncrementMultiplier()
    {
        beatMultiplier++;
        if (beatMultiplier > beatMultiplierIfHit + 1){
            beatMultiplier = beatMultiplierIfHit + 1;
        }
        Debug.Log($"✅ Combo hit! Multiplier: {beatMultiplier}");
    }
    private void BreakMultiplier()
    {
        beatMultiplier = Mathf.Max(1, beatMultiplier - 3);
        Debug.Log($"❌ Combo broken! Multiplier now: {beatMultiplier}");
    }


    public void OnBeat(EasyEvent audioEvent)
    {
        // lastBeatTime = Time.time;
        
        beatMultiplierIfHit++;
        Debug.Log($"BEAT {beatMultiplierIfHit}");
        // Start coroutine to check for missed hit 0.25s later
        StartCoroutine(CheckMultiplierMissAfterDelay(0.25f));
    }
    
}
