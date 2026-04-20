using UnityEngine;

public class GhostFireTrapVisual : MonoBehaviour
{
    public TriangleTrap targetTrap;
    
    [Header("Refs")]
    public SpriteRenderer sr;
    public Transform player;

    [Header("Distance")]
    public float showDistance = 6f;
    public float hideDistance = 7f;   // 比 showDistance 略大，防止边界抖动

    [Header("Cooldown")]
    public float recheckInterval = 0.1f; // 每隔多久检查一次距离
    public float visibleCooldown = 0.2f; // 显示后至少保持多久
    public float hiddenCooldown = 0.2f;  // 隐藏后至少保持多久

    [Header("State")]
    public bool isVisible;

    private float checkTimer;
    private float stateLockTimer;

    private void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            sr.enabled = false;
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null)
                player = ph.transform;
        }
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (stateLockTimer > 0f)
            stateLockTimer -= Time.deltaTime;

        if (checkTimer < recheckInterval) return;
        checkTimer = 0f;

        if (player == null) return;

        float sqrDist = (player.position - transform.position).sqrMagnitude;
        float showSqr = showDistance * showDistance;
        float hideSqr = hideDistance * hideDistance;

        if (!isVisible)
        {
            if (sqrDist <= showSqr && stateLockTimer <= 0f)
            {
                Show();
                stateLockTimer = visibleCooldown;
            }
        }
        else
        {
            if (sqrDist >= hideSqr && stateLockTimer <= 0f)
            {
                Hide();
                stateLockTimer = hiddenCooldown;
            }
        }
    }

    public void Show()
    {
        if (isVisible) return;

        isVisible = true;

        if (sr != null)
        {
            sr.enabled = true;

            if (GhostFireAnimManager.Instance != null)
                sr.sprite = GhostFireAnimManager.Instance.GetCurrentSprite();
        }

        if (GhostFireAnimManager.Instance != null)
            GhostFireAnimManager.Instance.Register(this);
    }

    public void Hide()
    {
        if (!isVisible) return;

        isVisible = false;

        if (sr != null)
            sr.enabled = false;

        if (GhostFireAnimManager.Instance != null)
            GhostFireAnimManager.Instance.Unregister(this);
    }

    public void RefreshSprite(Sprite sprite)
    {
        if (!isVisible || sr == null) return;
        sr.sprite = sprite;
    }
}