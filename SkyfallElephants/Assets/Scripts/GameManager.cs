using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public Action<GameState> OnGameStateChanged;

    [SerializeField] private GameState currentState = GameState.WaitingToStart;

    [SerializeField] private int lives = 3;

    public Action<int> OnLivesChanged;

    private void Awake()
    {
        if (i == null)
        {
            i = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
        OnLivesChanged?.Invoke(lives);
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        ResetBalls();
    }

    private void ResetBalls()
    {
        Ball[] ballsInScene = FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (ballsInScene.Length == 0) return;

        for (int i = 0; i < ballsInScene.Length; i++)
        {
            ballsInScene[i].RemoveBall();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentState)
            {
                case GameState.WaitingToStart:
                    StartGame();
                    break;
                case GameState.Playing:
                    SetGameState(GameState.Paused);
                    Time.timeScale = 0;
                    break;
                case GameState.Paused:
                    Time.timeScale = 1;
                    SetGameState(GameState.Playing);
                    break;
                case GameState.GameOver:
                    StartGame();
                    break;
                default:
                    break;
            }
        }
    }

    private void StartGame()
    {
        ScoreManager.i.ResetScore();
        ResetLives();
        SetGameState(GameState.Playing);
    }

    private void ResetLives()
    {
        lives = 3;
        OnLivesChanged?.Invoke(lives);
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    WaitingToStart,
    Playing,
    Paused,
    GameOver
}