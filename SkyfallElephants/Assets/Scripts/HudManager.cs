using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Color fullHeartColor = Color.red;
    [SerializeField] private Color emptyHeartColor = Color.gray;


    private void Start()
    {
        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
        GameManager.i.OnLivesChanged += GameManager_OnLivesChanged;
        ScoreManager.i.OnScoreChanged += ScoreManager_OnScoreChanged;

        ScoreManager_OnScoreChanged(0);
        UpdateHighScoreText(ScoreManager.i.GetHighScore());
    }

    private void GameManager_OnLivesChanged(int lives)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
            {
                hearts[i].color = fullHeartColor;
            }
            else
            {
                hearts[i].color = emptyHeartColor;
            }
        }
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
                break;
            case GameState.GameOver:
                infoText.text = "Press SPACE to Restart";
                UpdateHighScoreText(ScoreManager.i.GetHighScore());
                break;
            default:
                break;
        }
    }
}