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
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;
    public LayerMask wallLayer;

    [Header("Gap Check")]
    public Transform edgeCheck;
    public float edgeCheckDistance = 1f;

    [Header("Jump Logic")]
    public float jumpCooldown = 0.4f;
    public float playerHigherThreshold = 0.2f;

    [Header("Animation")]
    public Animator animator;

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

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        freezeTimer = freezeTime;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
            return;
        }

        CheckEnvironment();

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
            return;
        }

        if (!canChase || player == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
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
        SetMoveAnimation(Mathf.Abs(dirX) > 0.01f, dirX);

        if (!isGrounded) return;
        if (jumpCooldownTimer > 0f) return;

        if (isWallAhead)
        {
            Jump();
            return;
        }

        if (isGapAhead)
        {
            Jump();
            return;
        }

        if (deltaY > playerHigherThreshold)
        {
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
                wallLayer
            );

            isWallAhead = wallHit.collider != null;
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

    private void SetMoveAnimation(bool isMoving, float dirX)
    {
        if (animator == null) return;

        animator.SetBool("Move", isMoving);

        if (Mathf.Abs(dirX) > 0.01f)
        {
            animator.SetFloat("FaceX", dirX > 0f ? 1f : -1f);
        }
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
        SetMoveAnimation(false, 0f);
    }
}