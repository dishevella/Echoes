using UnityEngine;

public class Brother : MonoBehaviour, ISonarScannable
{
    [Header("Reference")]
    public TwinManager manager;
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 2f;
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.4f;
    public LayerMask wallLayer;

    [Header("Gap Check")]
    public Transform edgeCheck;
    public float edgeCheckDistance = 1f;

    [Header("Jump Logic")]
    public float jumpCooldown = 0.4f;

    [Header("Teleport")]
    public bool enableVerticalTeleport = true;
    public float verticalTeleportDistance = 4f;
    public Transform[] teleportPoints;
    public float teleportCooldown = 1f;

    [Header("Ignore Specific Wall")]
    public Collider2D ignoredWall;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private bool isDead = false;
    private bool canMove = false;
    private bool facingRight = true;

    private bool isGrounded;
    private bool isWallAhead;
    private bool isGapAhead;

    private float jumpCooldownTimer = 0f;
    private float teleportCooldownTimer = 0f;

    private Collider2D selfCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (selfCollider != null && ignoredWall != null)
        {
            Physics2D.IgnoreCollision(selfCollider, ignoredWall, true);
        }
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        if (manager == null) return;

        manager.MarkBroScanned();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            SetMoveAnimation(false, 0f);
            return;
        }

        CheckEnvironment();

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }

        if (teleportCooldownTimer > 0f)
        {
            teleportCooldownTimer -= Time.fixedDeltaTime;
        }

        if (manager == null || player == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
            return;
        }

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
            return;
        }

        if (!manager.BroScanned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            SetMoveAnimation(false, 0f);
            return;
        }

        if (TryTeleportToNearestPoint())
        {
            SetMoveAnimation(false, 0f);
            return;
        }

        ChasePlayer();
    }

    private bool TryTeleportToNearestPoint()
    {
        if (!enableVerticalTeleport) return false;
        if (player == null) return false;
        if (teleportPoints == null || teleportPoints.Length == 0) return false;
        if (teleportCooldownTimer > 0f) return false;

        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);
        if (verticalDistance < verticalTeleportDistance) return false;

        Transform nearestPoint = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Transform point in teleportPoints)
        {
            if (point == null) continue;

            float sqrDistance = (player.position - point.position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearestPoint = point;
            }
        }

        if (nearestPoint == null) return false;

        rb.linearVelocity = Vector2.zero;
        transform.position = nearestPoint.position;
        teleportCooldownTimer = teleportCooldown;
        return true;
    }

    private void ChasePlayer()
    {
        float deltaX = player.position.x - transform.position.x;
        float dirX = Mathf.Abs(deltaX) > 0.05f ? Mathf.Sign(deltaX) : 0f;

        if (dirX > 0f && !facingRight) Flip(true);
        else if (dirX < 0f && facingRight) Flip(false);

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
        scale.x = Mathf.Abs(scale.x) * (facingRight ? -1f : 1f);
        transform.localScale = scale;
    }

    private void SetMoveAnimation(bool isMoving, float dirX)
    {
        if (animator == null) return;

        animator.SetBool("Move", isMoving);

        if (Mathf.Abs(dirX) > 0.01f)
        {
            animator.SetFloat("FaceX", dirX > 0f ? -1f : 1f);
        }
    }

    public void StartMoving()
    {
        if (isDead) return;
        canMove = true;
    }

    public void StopMoving()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        SetMoveAnimation(false, 0f);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        SetMoveAnimation(false, 0f);
        gameObject.SetActive(false);
    }
}