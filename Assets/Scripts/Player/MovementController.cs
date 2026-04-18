using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public static MovementController instance;
    
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpSpeed = 15f;
    private bool canMove = true;

    private float moveInput;
    private bool isSprint = false;
    private bool isJump = false;

    [Header("GroundCheck")]
    private Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Stamina")]
    public Image staminaBar;
    public float consumeRate = 10;
    public float resumeRate = 5;

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
        if (!canMove) return;
        
        moveInput = 0;

        if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }        
        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
            

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isJump = true;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(staminaBar.fillAmount>0.01f)
            {
                isSprint = true;
            }
            else
            {
                isSprint = false;
            }

            if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
            {
                staminaBar.fillAmount -= consumeRate * Time.deltaTime;
            }
            else
            {
                staminaBar.fillAmount += resumeRate * Time.deltaTime;
            }
        }
        else
        {
            isSprint = false;
            staminaBar.fillAmount += resumeRate * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        float speed;
        if(!isSprint)
            speed = moveSpeed;
        else
            speed = sprintSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if(isJump)
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

    public void StopMove(float time)
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        Invoke("ResumeMove", time);
    }
    public void ResumeMove()
    {
        canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Prop"))
        {
            PropSO propSO = collision.GetComponent<Prop>().GetPropSO();
            BagSystem.instance.AddProp(propSO);
            Destroy(collision.gameObject);
        }
    }
}
