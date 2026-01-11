using UnityEngine;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallSO ballSO;

    [Header("Movement")]
    [SerializeField] private float horizontalAcceleration = 6f;

    [Header("Removal")]
    [SerializeField] private float shrinkDuration = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float targetHorizontalSpeed;
    private bool isRemoving = false;

    [SerializeField] private bool ease = false;

    [SerializeField] private float maxRotationSpeed = 270f;
    [SerializeField] private float minRotationSpeed = 90;
    private float rotationSpeed = 0f;
    private float rotationAngle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.value < 0.5f ? -1f : 1f);
    }

    public BallSO GetBallSO() => ballSO;

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

        spriteRenderer.sprite = ballSO.ballSprite;
        spriteRenderer.color = ballSO.ballColor;

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

        rotationAngle = (rotationAngle + rotationSpeed * Time.deltaTime) % 360f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
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
            if (!ballSO.winLife)
                GameManager.i.LoseLife();

            RemoveBall();
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
        {
            RemoveBall();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AudioManager.i.PlaySfx("Bounce");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRemoving) return;

        if (collision.CompareTag("Basket"))
        {
            ScoreManager.i.AddScore(ballSO.pointValue);
            if (ballSO.winLife)
            {
                GameManager.i.GainLife();
                AudioManager.i.PlaySfx("HeartGain");
            }
            else
                AudioManager.i.PlaySfx("Catch");

            
            RemoveBall();
        }
    }

    #endregion
}
