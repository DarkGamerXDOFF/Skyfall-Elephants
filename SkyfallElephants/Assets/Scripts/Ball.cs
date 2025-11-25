using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallSO ballSO;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private float moveForce = 5f;
    [SerializeField] private float forceAngle;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public static void CreateBall(BallSO ballSO, Vector2 position, float forceAngle)
    {
        Ball ball = Instantiate(ballSO.ballPf, position, Quaternion.identity).GetComponent<Ball>();
        ball.ballSO = ballSO;
        ball.forceAngle = forceAngle;
        ball.moveForce = ballSO.moveForce;

        ball.MoveObject();
    }

    private void MoveObject()
    {
        float angle = forceAngle * Mathf.Deg2Rad;
        rb.AddForce(new Vector2(Mathf.Cos(angle), -Mathf.Sin(angle)) * moveForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameManager.i.LoseLife();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Basket"))
        {
            ScoreManager.i.AddScore(ballSO.pointValue);
            Destroy(gameObject);
        }
    }
}
