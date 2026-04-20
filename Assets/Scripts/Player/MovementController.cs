using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public static MovementController instance;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Animation")]
    public Animator Anim;

    [Header("Movement")]
    public float moveSpeed = 10f;

    [Header("I Wanna Physics")]
    public float jumpVelocity = 16f;
    public float gravity = -40f;
    public float maxFallSpeed = -20f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.1f;     // 离地后还能跳
    public float jumpBufferTime = 0.1f; // 提前按跳

    [Header("Jump Count")]
    public int maxJumpCount = 2;
    private int currentJumpCount;
    private bool wasGrounded;

    //private float coyoteTimer;
    private float jumpBufferTimer;

    private bool canMove = true;
    private float moveInput;

    [Header("GroundCheck")]
    private Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Stamina")]
    public Image staminaBar;
    public float consumeRate = 10;
    public float resumeRate = 5;

    [Header("Wall Dection")]
    public PlayerWallLock wallLock;

    [Header("Interaction")]
    public GameObject currentInteractiveObject;

    [Header("Drop")]
    public float dropDownTime = 0.3f;
    public LayerMask dropLayerMask;
    private Collider2D playerCollider;
    private bool isDropping = false;

    [Header("Step Settings")]
    public float stepInterval = 0.35f;
    private float stepTimer;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        groundCheck = transform.Find("GroundCheck");

        currentJumpCount = maxJumpCount;
        wasGrounded = IsGrounded();
    }

    private void Update()
    {
        if (!canMove) return;

        moveInput = 0;

        bool canMoveLeft = !wallLock.TouchingLeftWall || wallLock.IsGrounded;
        bool canMoveRight = !wallLock.TouchingRightWall || wallLock.IsGrounded;

        if (Input.GetKey(KeyCode.A) && canMoveLeft)
        {
            moveInput = -1f;
            sr.flipX = true;
        }

        if (Input.GetKey(KeyCode.D) && canMoveRight)
        {
            moveInput = 1f;
            sr.flipX = false;
        }

        // ===== 地面检测 =====
        bool grounded = IsGrounded();

        // 只有“刚接地”时才恢复跳跃次数
        if (grounded && !wasGrounded)
        {
            currentJumpCount = maxJumpCount;
        }

        wasGrounded = grounded;

        // ===== Jump Buffer =====
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;
            
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        // ===== 执行跳跃 =====
        // 只要还有跳跃次数，就可以跳
        if (jumpBufferTimer > 0 && currentJumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);

            currentJumpCount--;
            jumpBufferTimer = 0;
            //coyoteTimer = 0;

            Anim.SetTrigger("Jump");
            PlayAudio.instance.PlayJump();
        }


        // ===== 动画 =====
        UpdateAnimation();

        if(Input.GetKeyDown(KeyCode.F) && currentInteractiveObject!=null)
        {
            if(currentInteractiveObject.TryGetComponent<PuzzleExampleController>(out var puzzleExampleController))
            {
                puzzleExampleController.Interact();
            }
            else if (currentInteractiveObject.TryGetComponent<PuzzleTrigger>(out var puzzleTrigger))
            {
               puzzleTrigger.Interact();
            }
            else if (currentInteractiveObject.TryGetComponent<KeyChecker>(out var keyChecker))
            {
                keyChecker.Interact();
            }
            else if (currentInteractiveObject.TryGetComponent<SonarRelayActivate>(out var sonarRelayActivate))
            {
                sonarRelayActivate.Interact();
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TryDropDown();
        }

        if (IsMove() && IsGrounded())
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayAudio.instance.PlayWalk();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        // ===== 水平移动（恒定速度）=====
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // ===== 手动重力 =====
        float newY = rb.linearVelocity.y + gravity * Time.fixedDeltaTime;

        if (newY < maxFallSpeed)
            newY = maxFallSpeed;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool IsMove()
    {
        return Mathf.Abs(rb.linearVelocity.x) > 0.01f;
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
        if (collision.gameObject.CompareTag("Prop"))
        {
            if (!BagSystem.instance.IsHaveProp(collision.gameObject.GetComponent<Prop>().GetPropSO()))
            {
                PropSO propSO = collision.GetComponent<Prop>().GetPropSO();
                BagSystem.instance.AddProp(propSO);
                Destroy(collision.gameObject);
                PlayAudio.instance.PlayPickup();
            }
            
         
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive"))
        {
            currentInteractiveObject = collision.gameObject;
            if (currentInteractiveObject.TryGetComponent<PuzzleTrigger>(out var puzzleTrigger))
            {
                puzzleTrigger.darkForm.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 100);
            }
            else if (currentInteractiveObject.TryGetComponent<PuzzleExampleController>(out var puzzleExampleController))
            {
                puzzleExampleController.darkForm.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 200);
            }
            else if (currentInteractiveObject.TryGetComponent<KeyChecker>(out var keyChecker))
            {
                keyChecker.close.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 100);
            }
            else if(currentInteractiveObject.TryGetComponent<SonarRelayActivate>(out var sonarRelayActivate))
            {
                sonarRelayActivate.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 150);
            }
            
        }
        if (collision.gameObject.CompareTag("Moveable"))
        {
            Anim.SetBool("IsPush", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactive"))
        {
            if (currentInteractiveObject == collision.gameObject)
            {
                currentInteractiveObject = null;
            }

            if (collision.TryGetComponent<PuzzleTrigger>(out var puzzleTrigger))
            {
                puzzleTrigger.darkForm.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
            else if (collision.TryGetComponent<PuzzleExampleController>(out var puzzleExampleController))
            {
                puzzleExampleController.darkForm.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
            else if (collision.TryGetComponent<KeyChecker>(out var keyChecker))
            {
                keyChecker.close.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
            else if (collision.TryGetComponent<SonarRelayActivate>(out var sonarRelayActivate))
            {
                sonarRelayActivate.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        }
        if (collision.gameObject.CompareTag("Moveable"))
        {
            Anim.SetBool("IsPush", false);
        }
    }

    void UpdateAnimation()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);

        Anim.SetFloat("Speed", speed);

        bool grounded = IsGrounded();
        //Anim.SetBool("IsGrounded", grounded);
    }

    private void TryDropDown()
    {
        if (isDropping) return;

        Collider2D platform = GetCurrentOneWayPlatform();
        if (platform != null)
        {
            StartCoroutine(DropDownRoutine(platform));
        }
    }

    private Collider2D GetCurrentOneWayPlatform()
    {
        Vector2 origin = transform.position;
        Vector2 size = new Vector2(0.8f, 0.1f);

        Collider2D hit = Physics2D.OverlapBox(origin + Vector2.down * 0.6f, size, 0f, dropLayerMask);
        if (hit != null && hit.GetComponent<PlatformEffector2D>() != null)
        {
            return hit;
        }

        return null;
    }

    private IEnumerator DropDownRoutine(Collider2D platform)
    {
        isDropping = true;

        Physics2D.IgnoreCollision(playerCollider, platform, true);

        yield return new WaitForSeconds(dropDownTime);

        Physics2D.IgnoreCollision(playerCollider, platform, false);

        isDropping = false;
    }
}