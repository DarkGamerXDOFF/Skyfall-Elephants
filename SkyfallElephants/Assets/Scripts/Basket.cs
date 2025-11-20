using UnityEngine;

public class Basket : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            ScoreManager.i.AddScore(1);
            Destroy(collision.gameObject);
        }
    }
}
