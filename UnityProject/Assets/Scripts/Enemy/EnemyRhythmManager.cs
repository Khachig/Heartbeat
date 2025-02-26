using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class EnemyRhythmManager : MonoBehaviour, IEasyListener
{
    public HealthSystem playerHealth;
    public float comboHealAmt;
    private EasyRhythmAudioManager audioManager;
    private List<GameObject> enemies = new List<GameObject>();

    // Number of bars the current sequence will flash for
    private int currSequenceLength = 1;
    private int beatsPerBar = 4;
    private int lastSequenceStartBar = 1;

    // List of arrows to flash on each beat in order
    // First elem in vector indicates which enemy to flash
    // Second elem in vector indicates which arrow of that enemy to flash
    // If the first elem is -1, do nothing on that beat
    private List<Vector2> sequenceTimings = new List<Vector2>();
    private int sequenceIndex = 0;
    private int sequenceNum = 1;
    private bool hasStartedCombo = false;
    private bool hasBrokenCombo = false;
    private int nextInComboIdx = 0;
    public int comboNum = 0;
    private float timeAtLastComboHit;

    private void Start()
    {
        PlayerAttack.onAttackMiss += OnAttackMiss;
        timeAtLastComboHit = Time.time;

        if (!audioManager)
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();
        audioManager.AddListener(this);
        beatsPerBar = audioManager.myAudioEvent.TimeSigAsArray()[0];
    }

    private void Update()
    {
        if (beatsPerBar == 0)
        {
            beatsPerBar = audioManager.myAudioEvent.TimeSigAsArray()[0];
            if (beatsPerBar > 0)
            {
                GenerateSequenceTimings();
            }
        }
    }

    public void AddEnemy(GameObject enemy) { enemies.Add(enemy); }

    public void RemoveEnemy(GameObject enemy) { 
        int enemyIndex = GetEnemyIndex(enemy);
        enemies[enemyIndex] = null;
        for (int i = 0; i < sequenceTimings.Count; i++)
        {
            if (sequenceTimings[i][0] == enemyIndex)
                sequenceTimings[i] = new Vector2(-1, sequenceTimings[i][1]);
        }
     }

    public void InitNewSequence()
    {
        GenerateSequenceTimings();
        ResetCombo();
    }

    public bool HasBrokenCombo () { return hasBrokenCombo; }

    public bool IsNextEnemyInCombo(GameObject enemy)
    {
        int nextEnemyInComboIdx = (int) sequenceTimings[nextInComboIdx][0];
        if (nextEnemyInComboIdx == -1 && enemy == null)
            return true;
        else if (nextEnemyInComboIdx >= 0)
            return enemy == enemies[nextEnemyInComboIdx];
        return false;
    }

    public void ContinueCombo()
    {
        if (!hasStartedCombo)
            hasStartedCombo = true;

        if (sequenceTimings[nextInComboIdx][0] >= 0)
        {
            comboNum++;
            Effects.SpecialEffects.ComboContinueEffect(comboNum);
        }

        nextInComboIdx++;
        timeAtLastComboHit = Time.time;
        if (nextInComboIdx == sequenceTimings.Count)
            OnComboComplete();
    }

    public void BreakCombo()
    {
        if (nextInComboIdx > 0)
            Effects.SpecialEffects.ComboBreakEffect();
        nextInComboIdx = 0;
        hasBrokenCombo = true;
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (audioEvent.CurrentBar >= lastSequenceStartBar + currSequenceLength)
        {
            lastSequenceStartBar = audioEvent.CurrentBar;
            sequenceIndex = 0;
            sequenceNum = 1;
        }

        if (sequenceTimings.Count == 0)
            return;
        

        Vector2 indices = sequenceTimings[sequenceIndex];
        if (!HasBrokenCombo() && indices[0] >= 0)
        {
            EnemyBehaviour enemy = enemies[(int)indices[0]].GetComponent<EnemyBehaviour>();
            enemy.FlashArrow((int)indices[1], sequenceNum);
        }
        sequenceIndex++;
        if (sequenceIndex < sequenceTimings.Count && sequenceTimings[sequenceIndex][0] >= 0)
            sequenceNum++;

        if (hasStartedCombo && !HasBrokenCombo() &&
            Time.time - timeAtLastComboHit > audioEvent.BeatLength() * 1.5f)
        {
            if (sequenceTimings[nextInComboIdx][0] == -1)
                ContinueCombo();
            else
                BreakCombo();
        }
    }

    private void GenerateSequenceTimings()
    {
        int beatsInSequence = 0;
        List<Vector2> newSequenceTimings = new List<Vector2>();
        enemies.RemoveAll(enemy => enemy == null);

        // Assume enemies are in sequence order
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyData data = enemies[i].GetComponent<EnemyData>();
            // One arrow enemies -> 1 arrow = 2 beats
            if (data.arrowArrangement.Length == 1)
            {
                newSequenceTimings.Add(new Vector2(i, 0));
                newSequenceTimings.Add(new Vector2(-1, 0));
                beatsInSequence += 2;
                continue;
            }

            // Multi arrow enemies -> 1 arrow = 1 beat
            for (int j = 0; j < data.arrowArrangement.Length; j++)
            {
                newSequenceTimings.Add(new Vector2(i, j));
                beatsInSequence++;
            } 
        }

        // Fill in rest of bar with rests
        while (beatsInSequence % beatsPerBar != 0)
        {
            newSequenceTimings.Add(new Vector2(-1, 0));
            beatsInSequence++;
        }

        currSequenceLength = beatsInSequence / beatsPerBar;
        sequenceTimings = newSequenceTimings;
        sequenceIndex = 0;
        sequenceNum = 1;
        lastSequenceStartBar = 1; // Reset this so sequence starts playing at next bar
    }

    private int GetEnemyIndex(GameObject enemy)
    {
        int ret = -1;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == enemy)
                return i;
        }
        return ret;
    }

    private void ResetCombo()
    {
        nextInComboIdx = 0;
        comboNum = 0;
        hasStartedCombo = false;
        hasBrokenCombo = false;
    }

    private void OnAttackMiss()
    {
        if (!HasBrokenCombo())
            BreakCombo();
    }

    private void OnComboComplete()
    {
        sequenceTimings.Clear();
        Effects.SpecialEffects.PlayerHealEffect();
        playerHealth.Heal(comboHealAmt);
    }
}
