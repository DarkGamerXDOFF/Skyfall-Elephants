using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private float moveInput;

    private Rigidbody2D rb;

    [SerializeField] private bool canMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameManager.i.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        canMove = state == GameState.Playing;
    }

    private void Update()
    {
        if (canMove)
            moveInput = Input.GetAxisRaw("Horizontal");
        else
            moveInput = 0f;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput, 0) * speed * Time.deltaTime;
    }
}
