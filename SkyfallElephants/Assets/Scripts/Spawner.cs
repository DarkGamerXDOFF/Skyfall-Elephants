using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    public BallSO[] ballSOs;

    [SerializeField] private float spawnInterval = 1f;
    private float timer;

    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnRangeX = 4f;

    [SerializeField] private Vector2 maxForceAngles;

    private void Update()
    {
        if (timer >= spawnInterval)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight);
            float angle = Random.Range(maxForceAngles.x, maxForceAngles.y);
            BallSO ball = ballSOs[Random.Range(0, ballSOs.Length)];
            Ball.CreateBall(ball, spawnPos, angle);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector2(-spawnRangeX, spawnHeight), new Vector2(spawnRangeX, spawnHeight));
    }
}
