using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallSO ballSO;
    [SerializeField] private float horizontalSpeed = 1f;

    private Rigidbody2D rb;

    [SerializeField] private float shrinkDuration = 0.15f;

    private bool isRemoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public static Ball CreateBall(BallSO ballSO, Vector2 position, float horizontalSpeed)
    {
        Ball ball = Instantiate(ballSO.ballPf, position, Quaternion.identity).GetComponent<Ball>();
        ball.ballSO = ballSO;
        ball.horizontalSpeed = horizontalSpeed;

        ball.MoveObject();

        return ball;
    }

    public void RemoveBall()
    {
        if (isRemoving) return;
        isRemoving = true;

        rb.simulated = false; // stops physics cleanly
        StartCoroutine(ShrinkAndDestroy());
    }

    private void MoveObject()
    {
        rb.linearVelocity = new Vector2(horizontalSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRemoving) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            GameManager.i.LoseLife();
            RemoveBall();
        }
        if (collision.gameObject.CompareTag("Ceiling"))
        {
            RemoveBall();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRemoving) return;

        if (collision.CompareTag("Basket"))
        {
            ScoreManager.i.AddScore(ballSO.pointValue);
            AudioManager.i.PlaySfx("Catch");
            RemoveBall();
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}
