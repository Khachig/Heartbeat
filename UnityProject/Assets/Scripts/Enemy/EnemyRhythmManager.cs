using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using FMODUnity;

public class EnemyRhythmManager : MonoBehaviour, IEasyListener
{
    public delegate void OnEnterAttackPhase();
    public OnEnterAttackPhase onEnterAttackPhase;
    public delegate void OnExitAttackPhase();
    public OnExitAttackPhase onExitAttackPhase;
    public HealthSystem playerHealth;
    public EasyRhythmAudioManager audioManager;
    public float comboHealAmt;
    public EventReference ComboHit;
    public EventReference ComboFail;

    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> arrows = new List<GameObject>();
    private bool hasStartedCombo = false;
    private bool hasBrokenCombo = false;
    private int comboNum = 0;
    private float timeAtLastComboHit;
    private float timeToJudgementLine = 0f; // How long projectiles should travel before hitting judgement line

    private bool isProjectilePhase = true;
    private bool hasStartedRhythmSequence = false;
    private List<int> rhythmSequence = null;
    private int rhythmSequenceIdx = 0;
    private int lastSequencePlayedBar = 0;
    
    private int numBossWaves = 6;
    private int currBossWave = 1;
    private int difficulty = 0; // 0 = easy; 1 = normal, -1 = tutorial
    private int wave = -2;

    private void Start()
    {
        PlayerAttack.onAttackMiss += OnAttackMiss;
        timeAtLastComboHit = Time.time;
        JudgementLine.DisableJudgementLine();

        if (!audioManager)
        {
            audioManager = GameObject.Find("EasyRhythmAudioManager").GetComponent<EasyRhythmAudioManager>();
            audioManager.AddListener(this);
        }
    }

    private void OnDisable()
    {
        PlayerAttack.onAttackMiss -= OnAttackMiss;
    }

    public void Reset()
    {
        lastSequencePlayedBar = 0;
        currBossWave = 1;
        rhythmSequenceIdx = 0;
        rhythmSequence = null;
        hasStartedCombo = false;
        hasBrokenCombo = false;
        comboNum = 0;
        timeAtLastComboHit = Time.time;
        timeToJudgementLine = 0f;
        isProjectilePhase = true;
        hasStartedRhythmSequence = false;
    }

    public void SetDifficulty(int diff) { difficulty = diff; }

    public void SetWave(int w) { 
        wave = w; 
    }

    public void AddEnemy(GameObject enemy) {
        enemies.Add(enemy);
        enemy.GetComponent<EnemyBehaviour>().SetTimeToJudgementLine(timeToJudgementLine);
    }

    public void RemoveEnemy(GameObject enemy) { enemies.Remove(enemy); }

    public void AddArrow(GameObject arrow) {
        if (arrows.Count == 0)
        {
            int lane = GetLaneFromArrow(arrow);
            JudgementLine.ActivateIndicatorAtLane(lane);
        }

        arrows.Add(arrow);
    }

    public void RemoveArrow(GameObject arrow) {
        arrows.Remove(arrow);
        if (arrows.Count == 0)
        {
            JudgementLine.DeactivateIndicators();
        }
        else
        {
            GameObject nextArrow = arrows[0];
            int lane = GetLaneFromArrow(nextArrow);
            JudgementLine.ActivateIndicatorAtLane(lane);
        }
    }

    public void InitNewSequence()
    {
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

    public bool HandlePlayerAttack(Vector2 input)
    {
        if (arrows.Count == 0)
            return true;       

        GameObject nextArrow = arrows[0];
        ArrowProjectileMovement nextArrowMovement = nextArrow.GetComponent<ArrowProjectileMovement>();

        if (!nextArrowMovement.IsInHitRange())
            return false;
        
        if ((input.y > 0 && nextArrow.name.Equals("UpArrowProjectile(Clone)")) ||
            (input.y < 0 && nextArrow.name.Equals("DownArrowProjectile(Clone)")) ||
            (input.x > 0 && nextArrow.name.Equals("RightArrowProjectile(Clone)")) ||
            (input.x < 0 && nextArrow.name.Equals("LeftArrowProjectile(Clone)"))
        )
        {
            ScoreManager.Instance.AddScore(50);
            RemoveArrow(nextArrow);
            nextArrowMovement.DestroyArrow();
            nextArrowMovement.GetParentEnemyBehaviour().HitEnemy();
            OnAttackSuccess();
            return true;
        }

        return false;
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

    public void KillAllEnemies()
    {
        foreach(GameObject enemy in enemies.ToList())
        {
            RemoveEnemy(enemy);
            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            enemyBehaviour.KillEnemy();
        }
    }

    public void OnBeat(EasyEvent audioEvent)
    {
        if (timeToJudgementLine == 0)
        {
            // Take a full bar to reach judgement line
            timeToJudgementLine = audioEvent.BeatLength() * audioEvent.TimeSigAsArray()[0];
        }

        if (enemies.Count == 0)
            return;

        HandleRhythmSequences(audioEvent);
    }

    private void HandleRhythmSequences(EasyEvent audioEvent)
    {
        if (audioEvent.CurrentBar == 1)
        {
            lastSequencePlayedBar = 0;
        }

        if (!hasStartedRhythmSequence && audioEvent.CurrentBar >= lastSequencePlayedBar + 2) // Starting new bar, start playing rhythm sequence
        {
            hasStartedRhythmSequence = true;
            if (wave == -2 || wave == -1){
                isProjectilePhase = false;
            }

            if (isProjectilePhase && IsBossWave())
                rhythmSequence = new List<int>() {1};
            else
            {
                if (difficulty == -1){
                    rhythmSequence = EnemyRhythms.GenerateTutorialRhythm();
                }
                else if (difficulty == 0)
                    rhythmSequence = EnemyRhythms.GenerateRandomEasyRhythm();
                else
                    rhythmSequence = EnemyRhythms.GenerateRandomRhythm();
            }
            rhythmSequenceIdx = 0;
            lastSequencePlayedBar = audioEvent.CurrentBar;
        }
        else if (hasStartedRhythmSequence &&
            // Ensure beat is 1 before rhythm sequence to give time for animations to play
            // Note: 1 "beat" in audioEvent is actually 1/2 a beat (8th note)
            // Dev decision to handle complex rhythms
            ((audioEvent.CurrentBar == lastSequencePlayedBar &&
              ((audioEvent.CurrentBeat == audioEvent.TimeSigAsArray()[0] - 1 &&
                rhythmSequence[rhythmSequenceIdx] == 1) ||
               (audioEvent.CurrentBeat == audioEvent.TimeSigAsArray()[0] &&
                rhythmSequence[rhythmSequenceIdx] == 2))) ||
             (audioEvent.CurrentBar == lastSequencePlayedBar + 1 && // Give sequences 1 bar gap between playing
              audioEvent.CurrentBeat == rhythmSequence[rhythmSequenceIdx] - 2) 
            )
           )
        {
            GameObject enemy = enemies[rhythmSequenceIdx % enemies.Count];
            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

            if (isProjectilePhase)
                enemyBehaviour.StartAttackAnim();
            else
                enemyBehaviour.StartArrowAttackAnim();

            rhythmSequenceIdx = (rhythmSequenceIdx + 1) % rhythmSequence.Count;
            
            if (IsBossWave())
                currBossWave++;

            if (!isProjectilePhase && !JudgementLine.IsEnabled())
            {
                JudgementLine.EnableJudgementLine();
                onEnterAttackPhase?.Invoke();
            }
        }
        else if (hasStartedRhythmSequence &&
                 (isProjectilePhase ||
                  (IsBossWave() && currBossWave < numBossWaves)) &&
                 audioEvent.CurrentBar > lastSequencePlayedBar + 1) // finished projectile phase?
        {
            isProjectilePhase = !isProjectilePhase;
            if (wave == 0 && audioEvent.CurrentBar <= 15 ){
                isProjectilePhase = true;
            }
            else if (wave == 0){
                wave = 1;
            }
            hasStartedRhythmSequence = false;
        }
        else if (hasStartedRhythmSequence &&
                 audioEvent.CurrentBar > lastSequencePlayedBar + 2)
        {
            KillAllEnemies();
            isProjectilePhase = true;
            JudgementLine.DisableJudgementLine();
            onExitAttackPhase?.Invoke();
            hasStartedRhythmSequence = false;
            currBossWave = 1;

            if (hasStartedCombo && !HasBrokenCombo())
                OnComboComplete();
        }
    }

    private bool IsBossWave()
    {
        if (enemies.Count == 1)
        {
            BossBehaviour bh = enemies[0].GetComponent<BossBehaviour>();
            if (bh)
                return true;
        }
        return false;
    }

    private int GetLaneFromArrow(GameObject arrow)
    {
        if (arrow.name.Equals("DownArrowProjectile(Clone)"))
            return 0;
        else if (arrow.name.Equals("RightArrowProjectile(Clone)"))
            return 1;
        else if (arrow.name.Equals("UpArrowProjectile(Clone)"))
            return 2;
        else if (arrow.name.Equals("LeftArrowProjectile(Clone)"))
            return 3;
        else
            // Shouldn't be here
            return -1;
    }
}
