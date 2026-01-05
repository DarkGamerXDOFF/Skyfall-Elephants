using UnityEngine;

public class Spawner : MonoBehaviour
{
    private enum SpawnMode { TimeInterval, Queue }
    [SerializeField] private SpawnMode spawnMode;

    [Header("Ball Setup")]
    [SerializeField] private BallSO[] ballSOs;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 1f;
    private float timer;

    [Header("Spawn Area")]
    [SerializeField] private float spawnHeight = 9f;
    [SerializeField] private float spawnRangeX = 4f;

    [Header("Horizontal Drift")]
    [SerializeField] private float baseDrift = 1f;
    [SerializeField] private float rareDriftMultiplier = 2.5f;
    [SerializeField, Range(0f, 1f)] private float rareDriftChance = 0.2f;

    private bool canSpawn = false;
    private Ball activeBall;

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
        Vector2 spawnPos = new Vector2(
            Random.Range(-spawnRangeX, spawnRangeX),
            spawnHeight
        );

        BallSO ballSO = ballSOs[Random.Range(0, ballSOs.Length)];

        float drift = Random.Range(-baseDrift, baseDrift);
        if (Random.value < rareDriftChance)
        {
            drift *= rareDriftMultiplier;
        }

        return Ball.CreateBall(ballSO, spawnPos, drift);
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
