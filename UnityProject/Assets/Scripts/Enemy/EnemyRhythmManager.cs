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
    public int comboNum = 0;
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
    private int rhythmSequencesPlayed = 0;
    private int waveStartBar = 0;

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

    public void SetDifficulty(int diff) { 
        difficulty = diff; 
        Debug.Log(difficulty);
        }

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
        //ResetCombo();
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
        comboNum = 0;
    } 

    public bool HandlePlayerAttack(Vector2 input)
    {
        if (arrows.Count == 0)
            return true;       

        GameObject nextArrow = arrows[0];
        ArrowProjectileMovement nextArrowMovement = nextArrow.GetComponent<ArrowProjectileMovement>();

        if (!nextArrowMovement.IsInHitRange())
            return false;
        
        if ((input.y > 0 && nextArrowMovement.GetArrowDirection() == ArrowDirection.UP) ||
            (input.y < 0 && nextArrowMovement.GetArrowDirection() == ArrowDirection.DOWN) ||
            (input.x > 0 && nextArrowMovement.GetArrowDirection() == ArrowDirection.RIGHT) ||
            (input.x < 0 && nextArrowMovement.GetArrowDirection() == ArrowDirection.LEFT)
        )
        {
            ScoreManager.Instance.AddRhythmScore();
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

    public void ResetCombo()
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
        GameObject playerObject = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.IncrementMultiplier(1);
        if (!HasBrokenCombo())
            ContinueCombo();
    }

    private void OnComboComplete()
    {
        Effects.SpecialEffects.PlayerHealEffect();
        playerHealth.Heal(comboHealAmt);
        //ResetCombo();
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
        int beatsPerBar = audioEvent.TimeSigAsArray()[0]; // == 8 rn
        int currentBar = audioEvent.CurrentBar; // number of bars into the current track
        int currentBeat = audioEvent.CurrentBeat; // 1-8 and repeat
        int nextAttackBeat = (rhythmSequence != null && rhythmSequenceIdx < rhythmSequence.Count)
            ? rhythmSequence[rhythmSequenceIdx]
            : -1;

        // Reset if a new loop starts
        ResetSequenceIfBarOne(currentBar);

        // Check if it's time to start a new rhythm sequence
        if (ShouldStartNewSequence(currentBar))
        {
            StartNewRhythmSequence(currentBar);
        }
        // Check if it's time to trigger an attack animation (next attack in 2 beats)
        else if (ShouldTriggerRhythmAttack(currentBar, currentBeat, beatsPerBar, nextAttackBeat))
        {
            TriggerEnemyAttack(currentBar, currentBeat, beatsPerBar);
        }
        // Check if it's time to trigger an attack shot (shoot projectile now)
        else if (ShouldTriggerRhythmAttackShoot(currentBar, currentBeat, beatsPerBar, nextAttackBeat))
        {
            TriggerEnemyAttackShoot();
        }
        // Check if the phase should transition between melee and projectile
        else if (ShouldSwitchAttackPhase(currentBar))
        {
            SwitchAttackPhase(currentBar);
        }
        // Check if the enemy sequences have passed after playing enough sequences
        else if (ShouldEndAttackPhase(currentBar))
        {
            // kill enemies and reset
            EndAttackPhase();
        }
     }
 
     // --- Condition Functions ---
 
    private bool ShouldStartNewSequence(int currentBar)
    {
        return !hasStartedRhythmSequence && currentBar >= lastSequencePlayedBar + 1; // TODO change 1 to barsInEnemySequence
    }
 
    private bool ShouldTriggerRhythmAttack(int currentBar, int currentBeat, int beatsPerBar, int attackBeat)
    {
        return hasStartedRhythmSequence &&
            IsTwoBeatsBeforeNextRhythmHit(currentBar, currentBeat, beatsPerBar, attackBeat);
    }
    
    private bool ShouldTriggerRhythmAttackShoot(int currentBar, int currentBeat, int beatsPerBar, int attackBeat)
    {
        return hasStartedRhythmSequence &&
            IsOnBeatForNextRhythmHit(currentBar, currentBeat, beatsPerBar, attackBeat);
    }
 
    private bool IsTwoBeatsBeforeNextRhythmHit(int currentBar, int currentBeat, int beatsPerBar, int attackBeat)
    {
        if (attackBeat == -1) return false; // invalid state

        int nextAttackBar = lastSequencePlayedBar + 1;

        // If the attack is very early in the bar, trigger must happen in previous bar
        // if (attackBeat <= 2)
        //     attackBar += 1;

        int totalBeatsNow = currentBar * beatsPerBar + currentBeat;
        int totalBeatsToHit = nextAttackBar * beatsPerBar + attackBeat;

        return totalBeatsNow == totalBeatsToHit - 2;
    }
    
    private bool IsOnBeatForNextRhythmHit(int currentBar, int currentBeat, int beatsPerBar, int attackBeat)
    {
        if (attackBeat == -1) return false; // invalid state

        int nextAttackBar = lastSequencePlayedBar + 1;
        int totalBeatsNow = currentBar * beatsPerBar + currentBeat;
        int totalBeatsToHit = nextAttackBar * beatsPerBar + attackBeat;

        return totalBeatsNow == totalBeatsToHit;
    }
 
    private bool ShouldSwitchAttackPhase(int currentBar)
    {
        return hasStartedRhythmSequence &&
            (isProjectilePhase || (IsBossWave() && currBossWave < numBossWaves)) &&
            currentBar > lastSequencePlayedBar + 1; //&& !isProjectileTutorial(currentBar)
    }

    private bool ShouldEndAttackPhase(int currentBar)
    {
        return hasStartedRhythmSequence && currentBar > lastSequencePlayedBar + 2;
    }
 
    // --- Action Functions ---
 
    private void ResetSequenceIfBarOne(int currentBar)
    {
    if (currentBar == 1)
        lastSequencePlayedBar = 0;
    }
    // --


    private void StartNewRhythmSequence(int currentBar)
    {
        hasStartedRhythmSequence = true;

        if (IsArrowTutorial())
            isProjectilePhase = false;

        rhythmSequence = GenerateRhythmBasedOnDifficulty();
        rhythmSequenceIdx = 0;
        lastSequencePlayedBar = currentBar;
    }
    private bool IsArrowTutorial(){
        return wave == -1 || wave == 0;
    }

    private List<int> GenerateRhythmBasedOnDifficulty()
    {
        if (isProjectilePhase && IsBossWave())
            return new List<int>() { 1 };

        if (difficulty == -1){
            return EnemyRhythms.GenerateTutorialRhythm();
        }
        else{
            return EnemyRhythms.GenerateDifficultyMatchedRhythm(difficulty);
        }
    }

    private void TriggerEnemyAttack(int currentBar, int currentBeat, int beatsPerBar)
    {
        GameObject enemy = enemies[rhythmSequenceIdx % enemies.Count];
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        if (isProjectilePhase)
        {
            enemyBehaviour.StartAttackAnim();
            if (rhythmSequenceIdx == 0 && IsBossWave()){
                GameObject playerObject = GameObject.FindWithTag("Player");
                PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
                playerMovement.SetBeatOffLimitLaneSpawn(currentBar*beatsPerBar + currentBeat);
                SpawnRandomOffLimitLanes(2);
            }
        }
        else
        {
            if (rhythmSequenceIdx == 0){
                despawnAllOffLimitLanes();
            }
            enemyBehaviour.StartArrowAttackAnim(); // despawn all lanes when first arrow starts
            // TODO issue is lanes spawn too early...
            // fix is to spawn only after all
        }

        if (!isProjectilePhase && !JudgementLine.IsEnabled())
        {
            JudgementLine.EnableJudgementLine();
            onEnterAttackPhase?.Invoke();
        }
    }

    private void TriggerEnemyAttackShoot()
    {
        GameObject enemy = enemies[rhythmSequenceIdx % enemies.Count];
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();

        if (isProjectilePhase)
            enemyBehaviour.Attack();
        else if (rhythmSequenceIdx > 0 &&
                    rhythmSequence[rhythmSequenceIdx] - rhythmSequence[rhythmSequenceIdx - 1] == 1)
            // If notes are back to back, keep arrow direction the same for ease of hitting
            enemyBehaviour.ArrowAttack(true);
        else
            enemyBehaviour.ArrowAttack(false);

        rhythmSequenceIdx = (rhythmSequenceIdx + 1) % rhythmSequence.Count;
    }

    private void SwitchAttackPhase(int currentBar)
    {
        isProjectilePhase = !isProjectilePhase;
        if (isProjectileTutorial(currentBar)) // projectile tutorial
        {
            isProjectilePhase = true;
        }
        // else if (wave == 0)
        // {
        //     wave = 1;
        // }

        if (IsBossWave())
        {
            Debug.Log($"total boss waves: {numBossWaves}");
            currBossWave++;
            if (!isProjectilePhase)
            {
                despawnAllOffLimitLanes();
            }
        }

        hasStartedRhythmSequence = false;
    }

    private void despawnAllOffLimitLanes(){
        for (int i = 0; i < Stage.Lanes.GetNumLanes(); i++)
            Stage.Lanes.DeSpawnOffLimitLane(i);
    }

    private bool isProjectileTutorial(int currentBar){
        
        int projTutorialWave = -2;

        if (waveStartBar == 0){
            waveStartBar = currentBar;
        }
        int tutorialEndBar = waveStartBar+4;
        Debug.Log($"currentbar {currentBar} tutorialEndBar {tutorialEndBar}");
        return wave == projTutorialWave && currentBar <= tutorialEndBar;
    }

    private void SpawnRandomOffLimitLanes(int numSpawnLanes){
        List<int> laneNumbers = Enumerable.Range(0, Stage.Lanes.GetNumLanes()).ToList();

        // GameObject playerObject = GameObject.FindWithTag("Player");
        // PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
        // laneNumbers.Remove(playerMovement.currentLaneIndex); // don't spawn on player

        for (int i = 0; i < numSpawnLanes; i++){
            int randomIndex = Random.Range(0, laneNumbers.Count);
            int randomlane = laneNumbers[randomIndex];
            Stage.Lanes.SpawnOffLimitLane(randomlane);
            laneNumbers.Remove(randomlane);
        }
        }

    private void EndAttackPhase()
    {
        int totalRhythmSeqPerEnemy = 1;
        rhythmSequencesPlayed++; // Increment sequence counter

        if (rhythmSequencesPlayed >= totalRhythmSeqPerEnemy) // End phase only after 2 sequences
        {
            KillAllEnemies();
            isProjectilePhase = true;
            JudgementLine.DisableJudgementLine();
            onExitAttackPhase?.Invoke();
            hasStartedRhythmSequence = false;
            currBossWave = 1;
            rhythmSequencesPlayed = 0;
            for (int i = 0; i < Stage.Lanes.GetNumLanes(); i++)
                    Stage.Lanes.DeSpawnOffLimitLane(i);

            if (hasStartedCombo && !HasBrokenCombo())
                OnComboComplete();
        }
        else
        {
            hasStartedRhythmSequence = false; // Prepare for another sequence
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
        ArrowProjectileMovement arrowMovement = arrow.GetComponent<ArrowProjectileMovement>();
        if (arrowMovement.GetArrowDirection() == ArrowDirection.DOWN)
            return 0;
        else if (arrowMovement.GetArrowDirection() == ArrowDirection.RIGHT)
            return 1;
        else if (arrowMovement.GetArrowDirection() == ArrowDirection.UP)
            return 2;
        else if (arrowMovement.GetArrowDirection() == ArrowDirection.LEFT)
            return 3;
        else
            // Shouldn't be here
            return -1;
    }
}
