using UnityEngine;

public class WoodenEnemy : MonoBehaviour, ISonarScannable
{
    [Header("Move")]
    public float moveSpeed = 2f;
    public float jumpForce = 8f;

    [Header("Freeze")]
    public float freezeTime = 1f;

    [Header("Target")]
    public Transform player;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;

    [Header("Gap Check")]
    public Transform edgeCheck;
    public float edgeCheckDistance = 0.5f;

    [Header("Jump Logic")]
    public float jumpCooldown = 0.4f;
    public float playerHigherThreshold = 0.8f;
    public float gapJumpThreshold = 2.5f;

    private Rigidbody2D rb;
    private float freezeTimer = 0f;
    private float jumpCooldownTimer = 0f;

    private bool isDead = false;
    private bool canChase = false;
    private bool facingRight = true;

    private bool isGrounded;
    private bool isWallAhead;
    private bool isGapAhead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        freezeTimer = freezeTime;
    }

    private void Update()
    {
        if (isDead) return;

        CheckEnvironment();

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (!canChase || player == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        float deltaX = player.position.x - transform.position.x;
        float deltaY = player.position.y - transform.position.y;

        float dirX = Mathf.Abs(deltaX) > 0.05f ? Mathf.Sign(deltaX) : 0f;

        if (dirX > 0f && !facingRight)
        {
            Flip(true);
        }
        else if (dirX < 0f && facingRight)
        {
            Flip(false);
        }

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);

        Debug.Log($"Grounded:{isGrounded} Wall:{isWallAhead} Gap:{isGapAhead} deltaY:{deltaY} cooldown:{jumpCooldownTimer}");

        if (!isGrounded) return;
        if (jumpCooldownTimer > 0f) return;

        if (isWallAhead)
        {
            Debug.Log("Jump because wall");
            Jump();
            return;
        }

        if (isGapAhead && Mathf.Abs(deltaX) <= gapJumpThreshold)
        {
            Debug.Log("Jump because gap");
            Jump();
            return;
        }

        if (deltaY > playerHigherThreshold)
        {
            Debug.Log("Jump because player higher");
            Jump();
            return;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCooldownTimer = jumpCooldown;
    }

    private void CheckEnvironment()
    {
        if (groundCheck != null)
        {
            Collider2D groundHit = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );

            isGrounded = groundHit != null;

            Debug.Log("Ground Hit = " + (groundHit != null ? groundHit.name : "NONE"));
        }
        else
        {
            isGrounded = false;
        }

        Vector2 faceDir = facingRight ? Vector2.right : Vector2.left;

        if (wallCheck != null)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(
                wallCheck.position,
                faceDir,
                wallCheckDistance,
                groundLayer
            );

            isWallAhead = wallHit.collider != null;
            Debug.Log("Wall Hit = " + (wallHit.collider != null ? wallHit.collider.name : "NONE"));
        }
        else
        {
            isWallAhead = false;
        }

        if (edgeCheck != null)
        {
            RaycastHit2D edgeHit = Physics2D.Raycast(
                edgeCheck.position,
                Vector2.down,
                edgeCheckDistance,
                groundLayer
            );

            isGapAhead = edgeHit.collider == null;
            Debug.Log("Edge Hit = " + (edgeHit.collider != null ? edgeHit.collider.name : "NONE"));
        }
        else
        {
            isGapAhead = false;
        }
    }

    private void Flip(bool faceRight)
    {
        facingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    public void StartChasing()
    {
        if (isDead) return;
        canChase = true;
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = facingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }

        if (edgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(edgeCheck.position, edgeCheck.position + Vector3.down * edgeCheckDistance);
        }
    }
}