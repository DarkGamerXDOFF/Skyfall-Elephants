using UnityEngine;

public class Spawner : MonoBehaviour
{

    private enum SpawnMode { timeInterval, queue}
    [SerializeField] private SpawnMode spawnMode;

    public BallSO[] ballSOs;

    [SerializeField] private float spawnInterval = 1f;
    private float timer;

    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnRangeX = 4f;

    [SerializeField] private Vector2 maxForceAngles;

    [SerializeField] private bool canSpawn = false;

    private Ball activeBall;

    private void Start()
    {
        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        canSpawn = state == GameState.Playing;
    }

    private void Update()
    {
        if (!canSpawn) return;


        switch (spawnMode)
        {
            case SpawnMode.timeInterval:
                if (timer >= spawnInterval)
                {
                    Vector2 spawnPos = new Vector2(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight);
                    float angle = Random.Range(maxForceAngles.x, maxForceAngles.y);

                    BallSO ball = ballSOs[Random.Range(0, ballSOs.Length)];
                    Ball.CreateBall(ball, spawnPos, Random.Range(-5f, 5f));
                    timer = 0f;
                }
                else
                {
                    timer += Time.deltaTime;
                }
                break;
            case SpawnMode.queue:
                if (activeBall == null)
                {
                    Vector2 spawnPos = new Vector2(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight);
                    BallSO ball = ballSOs[Random.Range(0, ballSOs.Length)];
                    activeBall = Ball.CreateBall(ball, spawnPos, Random.Range(-5f, 5f));
                }
                break;
            default:
                break;
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector2(-spawnRangeX, spawnHeight), new Vector2(spawnRangeX, spawnHeight));
    }
}
