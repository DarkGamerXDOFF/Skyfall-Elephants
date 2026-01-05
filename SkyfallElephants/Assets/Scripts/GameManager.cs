using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public Action<GameState> OnGameStateChanged;

    [SerializeField] private GameState currentState = GameState.WaitingToStart;

    [SerializeField] private int lives = 3;

    public Action<int> OnLivesChanged;

    public static PlayerInputActions playerInputActions;

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

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += GameManager_Interact;
    }

    private void Start()
    {
        MenuManager.i.OpenMenu("Main");
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
        MenuManager.i.OpenMenu("Main");
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

    private void GameManager_Interact(InputAction.CallbackContext context)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentState)
            {
                case GameState.WaitingToStart:
                    StartGame();
                    break;
                case GameState.Playing:
                    SetPause(true);
                    break;
                case GameState.Paused:
                    SetPause(false);
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
        MenuManager.i.OpenMenu("Game");
        ResetLives();
        SetGameState(GameState.Playing);
    }

    private void ResetLives()
    {
        lives = 3;
        OnLivesChanged?.Invoke(lives);
    }

    public void SetPause(bool isPaused)
    {
        if (isPaused)
        {
            MenuManager.i.OpenMenu("Pause");
            SetGameState(GameState.Paused);
            Time.timeScale = 0;
        }
        else
        {
            MenuManager.i.OpenMenu("Game");
            Time.timeScale = 1;
            SetGameState(GameState.Playing);
        }
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