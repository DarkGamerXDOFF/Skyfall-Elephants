using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager i;

    [SerializeField] private int score = 0;

    [SerializeField] private int highScore = 0;

    public Action<int> OnScoreChanged;

    [SerializeField] private bool updateHighScore;

    [SerializeField] private ParticleSystem[] confettiPS;

    private void Awake()
    {
        if (i == null) i = this;
        else Destroy(gameObject);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void PlayConfetti()
    {
        if (confettiPS != null && confettiPS.Length > 0)
        {
            foreach (ParticleSystem ps in confettiPS)
            {
                ps.Play();
            }
        }
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            CheckHighScore();
            updateHighScore = false;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);

        if (score > highScore && !updateHighScore)
        {
            if (highScore > 0)
            {
                PlayConfetti();
                AudioManager.i.PlaySfx("Highscore");
            }
            updateHighScore = true;
        }
    }

    public void CheckHighScore()
    {
        if (updateHighScore)
        {
            highScore = score;
            //Save high score to persistent storage 
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public int GetHighScore()
    {
        //Load the current highest score from persistent storage
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        return highScore;
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetHighscore()
    {
        highScore = 0;
        PlayerPrefs.SetInt("HighScore", highScore);
        GameManager.i.InfoSeen = false;
        PlayerPrefs.Save();
    }
}
