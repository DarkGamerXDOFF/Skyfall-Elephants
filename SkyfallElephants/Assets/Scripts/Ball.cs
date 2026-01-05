using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallSO ballSO;

    [Header("Movement")]
    [SerializeField] private float maxHorizontalSpeed = 3f;
    [SerializeField] private float horizontalAcceleration = 6f;

    [Header("Removal")]
    [SerializeField] private float shrinkDuration = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float targetHorizontalSpeed;
    private bool isRemoving = false;

    [SerializeField] private bool ease = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Factory method
    public static Ball CreateBall(BallSO ballSO, Vector2 position, float initialHorizontalDirection)
    {
        GameObject obj = Instantiate(GameAssets.i.ballPf, position, Quaternion.identity);
        Ball ball = obj.GetComponent<Ball>();

        ball.ballSO = ballSO;

        ball.InitializeFromSO();

        ball.SetInitialHorizontalDirection(initialHorizontalDirection);

        return ball;
    }

    private void InitializeFromSO()
    {
        rb.gravityScale = ballSO.gravityScale;
        transform.localScale *= ballSO.scaleMultiplier;
        spriteRenderer.color = ballSO.ballColor;   

        maxHorizontalSpeed = ballSO.maxHorizontalSpeed;
        horizontalAcceleration = ballSO.horizontalAcceleration;
    }

    public void SetInitialHorizontalDirection(float direction)
    {
        targetHorizontalSpeed = Mathf.Sign(direction) * ballSO.maxHorizontalSpeed;
    }

    private void FixedUpdate()
    {
        if (isRemoving) return;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = Mathf.MoveTowards(
            velocity.x,
            targetHorizontalSpeed,
            horizontalAcceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = velocity;
    }

    #region Removal Logic

    public void RemoveBall()
    {
        if (isRemoving) return;
        isRemoving = true;

        rb.simulated = false;
        StartCoroutine(ShrinkAndDestroy());
    }

    private System.Collections.IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < shrinkDuration)
        {
            float t = time / shrinkDuration;
            // Ease-out for better feel
            if (ease)
                t = 1f - Mathf.Pow(1f - t, 2f);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion

    #region Collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRemoving) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            GameManager.i.LoseLife();
            RemoveBall();
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
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

    #endregion
}
