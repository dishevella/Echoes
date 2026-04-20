using UnityEngine;

public class DashEnemy : MonoBehaviour, ISonarScannable
{
    [Header("Charge")]
    public float chargeTime = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 8f;
    public float dashTime = 0.4f;

    [Header("Cooldown")]
    public float cooldownTime = 2f;

    [Header("Dash Option")]
    public bool oneTimeTrigger = false;      // 勾上后，这个怪一生只冲一次
    public bool verticalOnlyDash = false;    // 勾上后，只能上下冲刺
    public bool horizontalOnlyDash = false;  // 勾上后，只能左右冲刺

    [Header("Target")]
    public Transform player;

    private Rigidbody2D rb;

    private bool isCharging = false;
    private bool isDashing = false;
    private bool hasTriggered = false;

    private float chargeTimer = 0f;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnSonarScanned()
    {
        if (isCharging || isDashing) return;
        if (cooldownTimer > 0f) return;
        if (player == null) return;
        if (oneTimeTrigger && hasTriggered) return;

        Vector2 dir = player.position - transform.position;

        // 如果两个都勾上，这里我设定为优先水平冲刺
        if (horizontalOnlyDash)
        {
            float dirX = dir.x >= 0f ? 1f : -1f;
            dashDirection = new Vector2(dirX, 0f);
        }
        else if (verticalOnlyDash)
        {
            float dirY = dir.y >= 0f ? 1f : -1f;
            dashDirection = new Vector2(0f, dirY);
        }
        else
        {
            dashDirection = dir.normalized;
        }

        isCharging = true;
        chargeTimer = chargeTime;
        cooldownTimer = cooldownTime;

        if (oneTimeTrigger)
        {
            hasTriggered = true;
        }
    }

    private void FixedUpdate()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }

        if (isCharging)
        {
            rb.linearVelocity = Vector2.zero;

            chargeTimer -= Time.fixedDeltaTime;
            if (chargeTimer <= 0f)
            {
                isCharging = false;
                isDashing = true;
                dashTimer = dashTime;
            }

            return;
        }

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }

        rb.linearVelocity = Vector2.zero;
    }
}