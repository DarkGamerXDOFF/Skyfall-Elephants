using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class HudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI highScoreText;


    private void Start()
    {
        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.i.OnLivesChanged += GameManager_OnLivesChanged;
        ScoreManager.i.OnScoreChanged += ScoreManager_OnScoreChanged;

        ScoreManager_OnScoreChanged(0);
        UpdateHighScoreText(0);
    }

    private void GameManager_OnLivesChanged(int lives)
    {
        livesText.text = $"Lives:{lives}";
    }

    private void ScoreManager_OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
    }

    private void UpdateHighScoreText(int score)
    {
        if (score <= 0)
            highScoreText.text = "";
        else
            highScoreText.text = $"Best: {score}";
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.WaitingToStart:
                infoText.text = "Press SPACE to Start";
                UpdateHighScoreText(0);
                break;
            case GameState.Playing:
                infoText.text = "";
                UpdateHighScoreText(0);
                break;
            case GameState.Paused:
                infoText.text = "Paused\nPress SPACE to Resume";
                break;
            case GameState.GameOver:
                infoText.text = "Game Over\nPress SPACE to Restart";
                UpdateHighScoreText(ScoreManager.i.GetHighScore());
                break;
            default:
                break;
        }
    }
}