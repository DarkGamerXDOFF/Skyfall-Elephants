using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public Action<GameState> OnGameStateChanged;

    [SerializeField] private GameState currentState = GameState.WaitingToStart;

    public int MaxLives => maxLives;
    [SerializeField] private int maxLives = 3;
    public int CurrentLives => currentLives;
    [SerializeField] private int currentLives = 3;

    public Action<int> OnLivesChanged;

    public static PlayerInputActions playerInputActions;

    [SerializeField] private bool infoSeen = false; 

    public bool InfoSeen
    {
        get
        {
            int prefValue = PlayerPrefs.GetInt("InfoSeen", 0);
            infoSeen = prefValue == 1 ? true : false;
            return infoSeen;
        }
        set 
        {
            infoSeen = value;
            if (infoSeen)
                PlayerPrefs.SetInt("InfoSeen", 1);
            else
                PlayerPrefs.SetInt("InfoSeen", 0);
            PlayerPrefs.Save();
        }
        
    }

    private void Awake()
    {
        if (i == null)
            i = this;
        else
            Destroy(gameObject);

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += GameManager_Interact;
    }

    private void Start()
    {
        if (!InfoSeen)
            MenuManager.i.OpenMenu("Info");
        else
            MenuManager.i.OpenMenu("Main");
    }

    public void LoseLife()
    {
        currentLives--;
        if (currentLives <= 0)
        {
            GameOver();
        }
        else 
            AudioManager.i.PlaySfx("Fall");
        
        OnLivesChanged?.Invoke(currentLives);
    }

    public void GainLife()
    {
        if (currentLives >= maxLives) return;
        currentLives++;
        OnLivesChanged?.Invoke(currentLives);
    }

    public void GameOver()
    {
        AudioManager.i.PlaySfx("GameOver");
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
        AudioManager.i.PlaySfx("Interact");
    }

    private void StartGame()
    {
        ScoreManager.i.ResetScore();
        MenuManager.i.OpenMenu("Game");
        ResetLives();
        Spawner.i.InitializeQueue();
        SetGameState(GameState.Playing);
    }

    private void ResetLives()
    {
        currentLives = maxLives;
        OnLivesChanged?.Invoke(currentLives);
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