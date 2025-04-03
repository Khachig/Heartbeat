using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public float Score { get; private set; }
    public float timeInterval = 0.1f; // Time interval for score increase
    public float scorePerSecond = 10f;  // Score increase per interval
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text scoreChangeText;
    public float scoreChange;
    private Coroutine currentCoroutine;
    private int currentLevel = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        scoreChangeText.gameObject.SetActive(false);
        // InvokeRepeating(nameof(AddTimeScore), timeInterval, timeInterval);
    }

    public void SetScoreLevel(int lvl)
    {
        currentLevel = lvl;
        }


    public void AddScore(int amount)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        Score += amount;
        scoreChangeText.text = "+" + amount.ToString();
        scoreChangeText.color = Color.green;
        scoreChange = amount;
        scoreChangeText.gameObject.SetActive(true);
        // Debug.Log("New Score: " + Score);
        currentCoroutine = StartCoroutine(HideTextAfterDelay(2f));
    }

    public void AddRhythmScore()
    {
        if (currentLevel == 0)
        {
            AddScore(165);
        }

        if (currentLevel == 1)
        {
            AddScore(366);
        }
        
    }

    public void AddKillScore()
    {
        if (currentLevel == 0)
        {
            AddScore(400);
        }

        if (currentLevel == 1)
        {
            AddScore(435);
        }
        
    }

    public void AddBossScore()
    {
        if (currentLevel == 0)
        {
            AddScore(1200);
        }

        if (currentLevel == 1)
        {
            AddScore(5000);
        }
        
    }


    public void DecreaseScore(int amount)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            // amount += scoreChange;
        }
        Score -= amount;
        if (Score < 0){
            Score = 0;
        }
        scoreChangeText.text = "-" + amount.ToString();
        scoreChangeText.color = Color.red;
        scoreChangeText.gameObject.SetActive(true);
        scoreChange = amount;
        //Debug.Log("New Score: " + Score);
        currentCoroutine = StartCoroutine(HideTextAfterDelay(1f));
    }
    
    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreChangeText.gameObject.SetActive(false);
    }

    private void AddTimeScore()
    {
        Score += scorePerSecond*timeInterval;
        // Debug.Log("Score increased over time: " + Score);
    }

    public void ResetScore()
    {
        Score = 0f;
    }

    private void Update()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.Score;
        finalScoreText.text = scoreText.text + "\n/40000";
    }

    public void ReInitScore()
    {
        ResetScore();
        scoreText.text = "Score: " + ScoreManager.Instance.Score;
        scoreChangeText.gameObject.SetActive(false);
        InvokeRepeating(nameof(AddTimeScore), timeInterval, timeInterval);
    }
}
