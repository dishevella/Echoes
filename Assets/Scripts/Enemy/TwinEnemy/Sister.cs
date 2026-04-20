using UnityEngine;

public class Sister : MonoBehaviour, ISonarScannable
{
    [Header("Reference")]
    public TwinManager manager;
    public Transform player;

    [Header("Move")]
    public float normalSpeed = 1.5f;
    public float linkedSpeed = 3f;

    private Rigidbody2D rb;
    private bool isDead = false;
    private bool canMove = false; // 新增：trigger后才开始动

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSonarScanned()
    {
        if (isDead) return;
        if (manager == null) return;

        manager.MarkSisScanned();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (manager == null || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 新增：没触发前不能动
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 被照到就完全不能动
        if (manager.SisScanned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float speed = manager.LinkActivated ? linkedSpeed : normalSpeed;

        float deltaX = player.position.x - transform.position.x;
        float dirX = deltaX >= 0f ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * speed, 0f);
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
    }

    public void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
    }
}