using UnityEngine;
using System;

public class SleepingBeauty : MonoBehaviour, ISonarScannable
{
    public Animator anim;
    
    [Header("Move")]
    public float moveSpeed = 2f;
    public bool startMoveRight = true;

    [Header("Wall Detect")]
    public Transform leftWallCheck;
    public Transform rightWallCheck;
    public float wallCheckDistance = 0.15f;
    public LayerMask wallLayer;

    [Header("State")]
    [SerializeField]private bool isActivated = false;
    [SerializeField]private bool isDead = false;
    [SerializeField]private float spwanDelay= 0.5f;
    [SerializeField]private bool canActivate;
    private float timer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int moveDir = 1; // 1 = right, -1 = left

    public PlayerTouch playerTouch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        moveDir = startMoveRight ? 1 : -1;
        UpdateFace();

        isActivated = false;
        canActivate = false;
        timer = 0;
    }

    private void Update()
    {
        if(timer<spwanDelay)
            timer += Time.deltaTime;
        else if (!canActivate)
        {
            canActivate = true;
        }
    }
    
    private void FixedUpdate()
    {
        if (isDead) return;

        if (!isActivated)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        CheckWallAndTurn();

        float vx = 0f;

        // 只有Y速度接近0时才允许X移动
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            vx = moveDir * moveSpeed;
        }

        rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    void CheckWallAndTurn()
    {
        Transform checkPoint = moveDir > 0 ? rightWallCheck : leftWallCheck;
        Vector2 dir = moveDir > 0 ? Vector2.right : Vector2.left;

        if (checkPoint == null) return;

        RaycastHit2D hit = Physics2D.Raycast(
            checkPoint.position,
            dir,
            wallCheckDistance,
            wallLayer
        );

        if (hit.collider != null)
        {
            TurnAround();
        }
    }

    void TurnAround()
    {
        moveDir *= -1;
        UpdateFace();
    }

    void UpdateFace()
    {
        if (sr != null)
        {
            sr.flipX = moveDir < 0;
        }
    }

    public void Activate()
    {
        if (isDead) return;
        if (!canActivate) return;
        isActivated = true;
        anim.SetTrigger("Activate");
    }

    public void LetDie()
    {
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void OnSonarScanned()
    {
        Activate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Die"))
        {
            LetDie();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (leftWallCheck != null)
        {
            Gizmos.DrawLine(
                leftWallCheck.position,
                leftWallCheck.position + Vector3.left * wallCheckDistance
            );
        }

        if (rightWallCheck != null)
        {
            Gizmos.DrawLine(
                rightWallCheck.position,
                rightWallCheck.position + Vector3.right * wallCheckDistance
            );
        }
    }
}