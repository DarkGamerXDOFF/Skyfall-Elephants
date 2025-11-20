using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform ballPf;

    [SerializeField] private float spawnInterval = 1f;
    private float timer;

    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnRangeX = 4f;

    private void Update()
    {
        if (timer >= spawnInterval)
        {
            Vector2 spawnPos = new Vector2(Random.Range(-spawnRangeX, spawnRangeX), spawnHeight);
            Instantiate(ballPf, spawnPos, Quaternion.identity);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Instantiate(ballPf, Mouse2D.GetMouseWorldPosition(), Quaternion.identity);
        //}
    }
}
