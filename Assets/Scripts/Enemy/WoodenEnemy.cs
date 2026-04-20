using UnityEngine;

public class WoodenEnemy : MonoBehaviour, ISonarScannable
{
    [Header("Move")]
    public float moveSpeed = 2f;

    [Header("Freeze")]
    public float freezeTime = 1f;

    [Header("Chase")]
    public bool startChasingOnAwake = false;

    [Header("Target")]
    public Transform player;

    private Rigidbody2D rb;
    private float freezeTimer = 0f;
    private bool canChase = false;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        canChase = startChasingOnAwake;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        if (!canChase) return;

        freezeTimer = freezeTime;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (!canChase)
        {
            StopHorizontalMove();
            return;
        }

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.fixedDeltaTime;
            StopHorizontalMove();
            return;
        }

        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (player == null)
        {
            StopHorizontalMove();
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float dirX = deltaX >= 0f ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
    }

    private void StopHorizontalMove()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void StartChasing()
    {
        if (isDead) return;
        canChase = true;
    }

    public void StopChasing()
    {
        canChase = false;
        StopHorizontalMove();
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }
}