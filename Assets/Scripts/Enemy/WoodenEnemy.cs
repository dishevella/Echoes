using UnityEngine;

public class WoodenEnemy : MonoBehaviour, ISonarScannable
{
    [Header("Move")]
    public float moveSpeed = 2f;

    [Header("Freeze")]
    public float freezeTime = 1f;

    [Header("Target")]
    public Transform player;

    private Rigidbody2D rb;
    private float freezeTimer = 0f;
    private bool isDead = false;
    private bool canChase = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSonarScanned()
    {
        if (isDead) return;

        freezeTimer = freezeTime;
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (!canChase)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float dirX = deltaX >= 0f ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, 0f);
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
}