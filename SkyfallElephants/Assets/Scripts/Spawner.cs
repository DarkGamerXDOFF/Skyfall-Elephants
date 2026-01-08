using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public static Spawner i;

    private enum SpawnMode { TimeInterval, Queue }
    [SerializeField] private SpawnMode spawnMode;

    [Header("Ball Setup")]
    [SerializeField] private BallSO[] ballSOs;
    [SerializeField] private BallSO lifeBallSO;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1f;
    private float timer;
    
    [Range(0, 1f)] [SerializeField]
    private float maxLifeSpawnChance = 0.1f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnHeight = 9f;
    [SerializeField] private float spawnRangeX = 4f;

    [Header("Horizontal Drift")]
    [SerializeField] private float baseDrift = 1f;
    [SerializeField] private float rareDriftMultiplier = 2.5f;
    [SerializeField, Range(0f, 1f)] private float rareDriftChance = 0.2f;

    private bool canSpawn = false;
    private Ball activeBall;

    [SerializeField] private Image[] ballQueueRenderers;
    private Queue<BallSO> nextBallSOs;

    private void Awake()
    {
        if (i == null) i = this;
        else Destroy(this);

        nextBallSOs = new Queue<BallSO>();
    }

    private void Start()
    {
        GameManager.i.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        canSpawn = state == GameState.Playing;
    }

    private void Update()
    {
        if (!canSpawn) return;

        switch (spawnMode)
        {
            case SpawnMode.TimeInterval:
                HandleTimedSpawn();
                break;

            case SpawnMode.Queue:
                HandleQueueSpawn();
                break;
        }
    }

    public void InitializeQueue()
    {
        nextBallSOs.Clear();

        for (int i = 0; i < ballQueueRenderers.Length; i++)
        {
            EnqueueRandomBall();
        }

        UpdateQueueUI();
    }

    private void EnqueueRandomBall()
    {
        BallSO randomBall = null;

        if (GameManager.i.CurrentLives < GameManager.i.MaxLives)
        {
            if (!nextBallSOs.Any(t => t.behavior == BallBehavior.Life))
            {
                if (Random.value <= maxLifeSpawnChance)
                {
                    randomBall = lifeBallSO;
                    nextBallSOs.Enqueue(randomBall);
                    return;
                }
            }
        }

        randomBall = ballSOs[Random.Range(0, ballSOs.Length)];
        nextBallSOs.Enqueue(randomBall);
    }

    private void HandleTimedSpawn()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;

        SpawnBall();
        timer = 0f;
    }

    private void HandleQueueSpawn()
    {
        if (activeBall == null)
            activeBall = SpawnBall();
    }

    private Ball SpawnBall()
    {
        BallSO ballSO = nextBallSOs.Dequeue();
        EnqueueRandomBall();
        UpdateQueueUI();

        bool otherSide = Random.value < 0.5f;

        Vector2 spawnPos = new Vector2(
            Random.Range(ballSO.minSpawnRange, ballSO.maxSpawnRange) * (otherSide ? -1 : 1),
            spawnHeight
        );
        
        float drift = Random.Range(-baseDrift, baseDrift);
        if (Random.value < rareDriftChance)
        {
            drift *= rareDriftMultiplier;
        }

        return Ball.CreateBall(ballSO, spawnPos, drift);
    }

    private void UpdateQueueUI()
    {
        for (int i = 0; i < ballQueueRenderers.Length; i++)
        {
            ballQueueRenderers[i].sprite = nextBallSOs.ToArray()[i].ballSprite;
            ballQueueRenderers[i].color = nextBallSOs.ToArray()[i].ballColor;
            ballQueueRenderers[i].transform.localScale = Vector3.one * nextBallSOs.ToArray()[i].scaleMultiplier / 1.5f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector2(-spawnRangeX, spawnHeight),
            new Vector2(spawnRangeX, spawnHeight)
        );
    }
}
