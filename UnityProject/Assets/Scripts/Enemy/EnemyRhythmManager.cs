using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using FMODUnity;

public class EnemyRhythmManager : MonoBehaviour, IEasyListener
{
    public HealthSystem playerHealth;
    public EasyRhythmAudioManager audioManager;
    public float comboHealAmt;
    public EventReference ComboHit;
    public EventReference ComboFail;

    private List<GameObject> enemies = new List<GameObject>();
    private bool hasStartedCombo = false;
    private bool hasBrokenCombo = false;
    private int comboNum = 0;
    private int maxComboNum = 0;
    private float timeAtLastComboHit;

    private void Start()
    {
        PlayerAttack.onAttackMiss += OnAttackMiss;
        PlayerAttack.onAttackSuccess += OnAttackSuccess;
        timeAtLastComboHit = Time.time;

        if (!audioManager)
        {
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();
            audioManager.AddListener(this);
        }
    }

    public void AddEnemy(GameObject enemy) { enemies.Add(enemy); }

    public void RemoveEnemy(GameObject enemy) { enemies.Remove(enemy); }

    public void InitNewSequence()
    {
        GenerateSequenceTimings();
        ResetCombo();
    }

    public bool HasBrokenCombo () { return hasBrokenCombo; }

    public void ContinueCombo()
    {
        if (!hasStartedCombo)
            hasStartedCombo = true;

        comboNum++;
        Effects.SpecialEffects.ComboContinueEffect(comboNum);
        FMOD.Studio.EventInstance comboHitInstance = RuntimeManager.CreateInstance(ComboHit);
        comboHitInstance.setParameterByName("ComboCount", comboNum);
        comboHitInstance.start();

        timeAtLastComboHit = Time.time;
    }

    public void BreakCombo()
    {
        Effects.SpecialEffects.ComboBreakEffect();
        hasBrokenCombo = true;
        RuntimeManager.PlayOneShot(ComboFail, transform.position);
    } 

    private void GenerateSequenceTimings()
    {
        int numArrows = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyData data = enemies[i].GetComponent<EnemyData>();
            numArrows += data.arrowArrangement.Length;
        }

        maxComboNum = numArrows;
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
        comboNum = 0;
        maxComboNum = 0;
        hasStartedCombo = false;
        hasBrokenCombo = false;
    }

    private void OnAttackMiss()
    {
        if (!HasBrokenCombo())
            BreakCombo();
    }

    private void OnAttackSuccess()
    {
        if (!HasBrokenCombo())
            ContinueCombo();
    }

    private void OnComboComplete()
    {
        Effects.SpecialEffects.PlayerHealEffect();
        playerHealth.Heal(comboHealAmt);
        ResetCombo();
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (hasStartedCombo && !HasBrokenCombo() && 
            Time.time - timeAtLastComboHit > audioEvent.BeatLength() * 1.5f)
        {
            if (enemies.Count == 0)
                OnComboComplete();
            else
            {
                if (enemies[0].GetComponent<BossBehaviour>() == null)
                    BreakCombo();
                else
                    OnComboComplete();
            }
        }
    }
}
