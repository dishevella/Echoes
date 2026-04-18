using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Detect")]
    public LayerMask detectLayer;
    public List<string> targetTagList = new List<string>();

    [Header("Per Target Cooldown")]
    public float targetCooldown = 3f;

    [Header("Ghost Fade")]
    public float ghostLifetime = 10f;
    public float startAlpha = 0.7f;

    [Header("Ghost Limit")]
    public int maxGhostCount = 50;

    [Header("Disable On Ghost")]
    public bool disableRigidbody2D = true;
    public bool disableCollider2D = true;
    public bool disableAnimator = true;
    public List<string> disableComponentNames = new List<string>();

    private CircleCollider2D circle;

    // 每个目标独立冷却
    private Dictionary<GameObject, float> lastGhostTime = new Dictionary<GameObject, float>();

    // 用来控制最大 ghost 数量
    private Queue<GameObject> activeGhosts = new Queue<GameObject>();

    // 上一帧/当前帧的声呐半径
    private float previousRadius = 0f;
    private float currentRadius = 0f;

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        currentRadius = GetWorldRadius();
        previousRadius = currentRadius;
    }

    void Update()
    {
        CleanupNullTargets();
        CleanupNullGhosts();

        previousRadius = currentRadius;
        currentRadius = GetWorldRadius();

        // 只在“正在向外扩张”时检测
        if (currentRadius > previousRadius + 0.001f)
        {
            ScanWaveFront(previousRadius, currentRadius);
        }
    }

    float GetWorldRadius()
    {
        if (circle == null) return 0f;

        // 2D 圆形通常取 X 即可；如果你可能非均匀缩放，可改成 Max(x,y)
        float worldScale = Mathf.Abs(transform.lossyScale.x);
        return circle.radius * worldScale;
    }

    void ScanWaveFront(float innerRadius, float outerRadius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, outerRadius, detectLayer);

        // 防止同一个 root 因多个子碰撞器重复处理
        HashSet<GameObject> processedRoots = new HashSet<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            GameObject target = hit.transform.root.gameObject;
            if (target == null) continue;

            if (processedRoots.Contains(target))
                continue;

            processedRoots.Add(target);

            if (!IsTargetTag(target.tag))
                continue;

            // 不扫 ghost 自己
            if (target.GetComponent<GhostFade>() != null)
                continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            // 只在“波前环带”内触发
            if (distance <= innerRadius || distance > outerRadius)
                continue;

            if (!CanCreateGhost(target))
                continue;

            CreateGhost(target);
            lastGhostTime[target] = Time.time;
        }
    }

    bool IsTargetTag(string tagName)
    {
        return targetTagList.Contains(tagName);
    }

    bool CanCreateGhost(GameObject target)
    {
        if (target == null) return false;

        if (!lastGhostTime.ContainsKey(target))
            return true;

        return Time.time - lastGhostTime[target] >= targetCooldown;
    }

    void CreateGhost(GameObject target)
    {
        GameObject ghost = Instantiate(target, target.transform.position, target.transform.rotation);
        ghost.transform.localScale = target.transform.localScale;

        // 防止被再次识别为目标
        ghost.tag = "Untagged";

        DisableSelectedComponents(ghost);

        if (disableRigidbody2D)
        {
            foreach (Rigidbody2D rb in ghost.GetComponentsInChildren<Rigidbody2D>(true))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }
        }

        if (disableCollider2D)
        {
            foreach (Collider2D col in ghost.GetComponentsInChildren<Collider2D>(true))
            {
                col.enabled = false;
            }
        }

        if (disableAnimator)
        {
            foreach (Animator anim in ghost.GetComponentsInChildren<Animator>(true))
            {
                anim.enabled = false;
            }
        }

        GhostFade fade = ghost.AddComponent<GhostFade>();
        fade.lifetime = ghostLifetime;
        fade.startAlpha = startAlpha;

        RegisterGhost(ghost);
    }

    void DisableSelectedComponents(GameObject ghost)
    {
        foreach (MonoBehaviour script in ghost.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (script == null) continue;
            if (script is GhostFade) continue;

            string typeName = script.GetType().Name;

            if (disableComponentNames.Contains(typeName))
            {
                script.enabled = false;
            }
        }
    }

    void RegisterGhost(GameObject ghost)
    {
        activeGhosts.Enqueue(ghost);

        while (activeGhosts.Count > maxGhostCount)
        {
            GameObject oldest = activeGhosts.Dequeue();
            if (oldest != null)
            {
                Destroy(oldest);
            }
        }
    }

    void CleanupNullTargets()
    {
        List<GameObject> toRemove = null;

        foreach (var kv in lastGhostTime)
        {
            if (kv.Key == null)
            {
                if (toRemove == null) toRemove = new List<GameObject>();
                toRemove.Add(kv.Key);
            }
        }

        if (toRemove != null)
        {
            foreach (GameObject key in toRemove)
            {
                lastGhostTime.Remove(key);
            }
        }
    }

    void CleanupNullGhosts()
    {
        if (activeGhosts.Count == 0) return;

        Queue<GameObject> newQueue = new Queue<GameObject>();

        while (activeGhosts.Count > 0)
        {
            GameObject g = activeGhosts.Dequeue();
            if (g != null)
            {
                newQueue.Enqueue(g);
            }
        }

        activeGhosts = newQueue;
    }

    void OnDrawGizmosSelected()
    {
        if (circle == null) circle = GetComponent<CircleCollider2D>();
        if (circle == null) return;

        Gizmos.color = Color.cyan;
        float r = Application.isPlaying ? currentRadius : GetPreviewRadius();
        Gizmos.DrawWireSphere(transform.position, r);
    }

    float GetPreviewRadius()
    {
        float worldScale = Mathf.Abs(transform.lossyScale.x);
        return circle.radius * worldScale;
    }
}