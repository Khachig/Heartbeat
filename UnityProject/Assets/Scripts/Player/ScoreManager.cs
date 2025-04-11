using UnityEngine;
using TMPro;
using System;
using System.Collections;
using FMODUnity;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public float Score { get; private set; }
    public float timeInterval = 0.1f; // Time interval for score increase
    public float scorePerSecond = 10f;  // Score increase per interval
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text finalRatingText;
    public TMP_Text scoreChangeText;
    public TMP_Text scoreMultiplierText;
    public float scoreChange;
    private Coroutine currentCoroutine;
    private int currentLevel = 0;
    private int beatMultiplier = 1;

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
        int changeAmount = (int)Math.Round((float)amount * (1.0f+ (float)(beatMultiplier/100)));
        Debug.Log($"beatMultiplier/100 {(float)beatMultiplier/100} {(1.0f + (float)(beatMultiplier/100))} {(float)amount * (1.0f+ (float)(beatMultiplier/100))}");
        Score += changeAmount;
        scoreChangeText.text = "+" + changeAmount.ToString();
        scoreChangeText.color = Color.green;
        scoreChange = changeAmount;
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
            AddScore(273);
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
        scoreText.text = "Score: " + AddCommasToScoreText();
        finalScoreText.text = "Final Score: \n" + AddCommasToScoreText();
        finalRatingText.text = "Rating: \n" + CalculateRating();
        scoreMultiplierText.text = "X"+ ScoreManager.Instance.beatMultiplier;
    }

    private String CalculateRating()
    {   
        float CurrentScore = ScoreManager.Instance.Score;
        // Calculate rating based on score
        if (CurrentScore >= 140000)
        {
            return "S";
        }
        else if (CurrentScore >= 110000)
        {
            return "A";
        }
        else if (CurrentScore >= 80000)
        {
            return "B";
        }
        else if (CurrentScore >= 50000)
        {
            return "C";
        }
        else if (CurrentScore >= 30000)
        {
            return "D";
        }
        else
        {
            return "F";
        }
    }

    // helper to add commas to score text
    private string AddCommasToScoreText()
    {
        // given ScoreManager.Instance.Score, return a string with commas
        string scoreString = ScoreManager.Instance.Score.ToString();
        int index = scoreString.IndexOf('.');
        if (index == -1)
        {
            index = scoreString.Length;
        }
        for (int i = index - 3; i > 0; i -= 3)
        {
            scoreString = scoreString.Insert(i, ",");
        }
        return scoreString;
    }

    public void ReInitScore()
    {
        ResetScore();
        scoreText.text = "Score: " + ScoreManager.Instance.Score;
        scoreChangeText.gameObject.SetActive(false);
        InvokeRepeating(nameof(AddTimeScore), timeInterval, timeInterval);
    }

    public void SetMultiplier(int amount)
    {
        beatMultiplier = amount;
        scoreMultiplierText.text = "X"+beatMultiplier;
    }

}
