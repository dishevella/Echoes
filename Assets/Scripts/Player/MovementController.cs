using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public static MovementController instance;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float sprintSpeed = 15f;
    public float jumpSpeed = 5f;

    private float moveInput;
    private bool isSprint = false;
    private bool isJump = false;

    [Header("GroundCheck")]
    private Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Stamina")]
    public Image staminaBar;   // 可以为空
    public float consumeRate = 10f;
    public float resumeRate = 5f;

    private bool HasStaminaBar()
    {
        return staminaBar != null;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");
    }

    private void Update()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;

        if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isJump = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // 如果没有 staminaBar，默认允许冲刺
            if (!HasStaminaBar() || staminaBar.fillAmount > 0.01f)
            {
                isSprint = true;
            }
            else
            {
                isSprint = false;
            }

            // 只有有 staminaBar 时，才处理消耗/恢复
            if (HasStaminaBar())
            {
                if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
                {
                    staminaBar.fillAmount = Mathf.Clamp01(
                        staminaBar.fillAmount - consumeRate * Time.deltaTime
                    );
                }
                else
                {
                    staminaBar.fillAmount = Mathf.Clamp01(
                        staminaBar.fillAmount + resumeRate * Time.deltaTime
                    );
                }
            }
        }
        else
        {
            isSprint = false;

            // 只有有 staminaBar 时，才恢复
            if (HasStaminaBar())
            {
                staminaBar.fillAmount = Mathf.Clamp01(
                    staminaBar.fillAmount + resumeRate * Time.deltaTime
                );
            }
        }
    }

    private void FixedUpdate()
    {
        float speed;

        if (!isSprint)
            speed = moveSpeed;
        else
            speed = sprintSpeed;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (isJump)
        {
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            isJump = false;
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool IsMove()
    {
        return rb.linearVelocity.magnitude > 0.01f;
    }
}