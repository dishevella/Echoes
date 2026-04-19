using UnityEngine;

public class Brother : MonoBehaviour, ISonarScannable
{
    [Header("Reference")]
    public TwinManager manager;
    public Transform player;

    [Header("Move")]
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        if (manager == null) return;

        manager.MarkBroScanned();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (manager == null || player == null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        
        if (!manager.BroScanned)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float dirX = deltaX >= 0f ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * moveSpeed, rb.linearVelocity.y);
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }
}