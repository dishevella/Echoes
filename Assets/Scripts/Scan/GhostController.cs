using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Detect")]
    public LayerMask detectLayer;
    public List<string> targetTagList = new List<string>();

    [Header("Per Target Cooldown")]
    public float targetCooldown = 2f;

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

    // 每个目标独立计时
    private Dictionary<GameObject, float> lastGhostTime = new Dictionary<GameObject, float>();

    // 控制最大 ghost 数量
    private Queue<GameObject> activeGhosts = new Queue<GameObject>();

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        CleanupNullTargets();
        CleanupNullGhosts();

        ScanAll();
    }

    void ScanAll()
    {
        float radius = circle.radius * Mathf.Abs(transform.lossyScale.x);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, detectLayer);

        HashSet<GameObject> processedRoots = new HashSet<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            // 🔥 用 collider 的 tag 判断
            if (!IsTargetTag(hit.tag))
                continue;

            GameObject root = hit.transform.root.gameObject;

            if (root == null) continue;

            // 防止一个物体多个 collider 被重复处理
            if (processedRoots.Contains(root))
                continue;

            processedRoots.Add(root);

            // 防止 ghost 自己再被扫描
            if (root.GetComponent<GhostFade>() != null)
                continue;

            if (!CanCreateGhost(root))
                continue;

            CreateGhost(root);
            lastGhostTime[root] = Time.time;
        }
    }

    bool IsTargetTag(string tagName)
    {
        return targetTagList.Contains(tagName);
    }

    bool CanCreateGhost(GameObject target)
    {
        if (!lastGhostTime.ContainsKey(target))
            return true;

        return Time.time - lastGhostTime[target] >= targetCooldown;
    }

    void CreateGhost(GameObject target)
    {
        GameObject ghost = Instantiate(target, target.transform.position, target.transform.rotation);
        ghost.transform.localScale = target.transform.localScale;

        ghost.tag = "Untagged";

        DisableSelectedComponents(ghost);

        if (disableRigidbody2D)
        {
            foreach (var rb in ghost.GetComponentsInChildren<Rigidbody2D>(true))
            {
                rb.simulated = false;
            }
        }

        if (disableCollider2D)
        {
            foreach (var col in ghost.GetComponentsInChildren<Collider2D>(true))
            {
                col.enabled = false;
            }
        }

        if (disableAnimator)
        {
            foreach (var anim in ghost.GetComponentsInChildren<Animator>(true))
            {
                anim.enabled = false;
            }
        }
        MakeGhostVisible(ghost);
        GhostFade fade = ghost.AddComponent<GhostFade>();
        fade.lifetime = ghostLifetime;
        fade.startAlpha = startAlpha;

        RegisterGhost(ghost);

    }

    void DisableSelectedComponents(GameObject ghost)
    {
        foreach (var script in ghost.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (script == null) continue;
            if (script is GhostFade) continue;

            string name = script.GetType().Name;

            if (disableComponentNames.Contains(name))
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
                Destroy(oldest);
        }
    }

    void MakeGhostVisible(GameObject ghost)
    {
        SpriteRenderer[] renderers = ghost.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr != null)
                sr.enabled = true;
        }
    }

    void CleanupNullTargets()
    {
        List<GameObject> removeList = new List<GameObject>();

        foreach (var kv in lastGhostTime)
        {
            if (kv.Key == null)
                removeList.Add(kv.Key);
        }

        foreach (var key in removeList)
        {
            lastGhostTime.Remove(key);
        }
    }

    void CleanupNullGhosts()
    {
        Queue<GameObject> newQueue = new Queue<GameObject>();

        while (activeGhosts.Count > 0)
        {
            GameObject g = activeGhosts.Dequeue();
            if (g != null)
                newQueue.Enqueue(g);
        }

        activeGhosts = newQueue;
    }


    void OnDrawGizmosSelected()
    {
        if (circle == null) circle = GetComponent<CircleCollider2D>();
        if (circle == null) return;

        Gizmos.color = Color.cyan;
        float r = circle.radius * Mathf.Abs(transform.lossyScale.x);
        Gizmos.DrawWireSphere(transform.position, r);
    }
}