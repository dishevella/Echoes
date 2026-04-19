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

    [Header("Target")]
    public Transform player;

    private Rigidbody2D rb;

    private bool isCharging = false;
    private bool isDashing = false;

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

        float deltaX = player.position.x - transform.position.x;
        float dirX = deltaX >= 0f ? 1f : -1f;
        dashDirection = new Vector2(dirX, 0f);

     
        isCharging = true;
        chargeTimer = chargeTime;

  
        cooldownTimer = cooldownTime;
    }

    private void FixedUpdate()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.fixedDeltaTime;
        }

        
        if (isCharging)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

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
            rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed, rb.linearVelocity.y);

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            return;
        }

       
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}