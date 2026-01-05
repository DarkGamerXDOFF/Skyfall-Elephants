using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private float moveInput;

    private Rigidbody2D rb;

    [SerializeField] private bool canMove;

    [SerializeField] private bool testingMode;

    [SerializeField] private Vector2 playerBoundries;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (testingMode)
            return;

        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        canMove = state == GameState.Playing || testingMode;
    }

    private void Update()
    {
        if (canMove)
            moveInput = GameManager.playerInputActions.Player.Movement.ReadValue<float>();
        else
            moveInput = 0f;

        rb.position = new Vector2(Mathf.Clamp(rb.position.x, playerBoundries.x, playerBoundries.y), rb.position.y);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput, 0) * speed * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(new Vector3(playerBoundries.x, 0, 0), 0.5f);
        Gizmos.DrawWireSphere(new Vector3(playerBoundries.y, 0, 0), 0.5f);
    }
}
