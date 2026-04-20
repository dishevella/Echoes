using UnityEngine;
using System.Collections.Generic;

public class SonarGhostScanner : MonoBehaviour
{
    [Header("Detect")]
    public LayerMask detectLayer;
    public List<string> targetTagList = new List<string>();

    [Header("Ghost Rule By Layer")]
    public List<string> staticLayerList = new List<string>();
    public List<string> dynamicLayerList = new List<string>();
    public List<string> visibleLayerList = new List<string>();
    public bool enableStaticTargetInsideMask = true;

    [Header("Visible Notify")]
    public float visibleNotifyCooldown = 0.5f;

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

    private Dictionary<GameObject, float> lastGhostTime = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> lastVisibleNotifyTime = new Dictionary<GameObject, float>();
    private Queue<GameObject> activeGhosts = new Queue<GameObject>();

    public void ScanAt(Vector2 center, float radius)
    {
        CleanupNullTargets();
        CleanupNullGhosts();
        CleanupNullVisibleNotifyTargets();

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, detectLayer);
        HashSet<GameObject> processedRoots = new HashSet<GameObject>();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (!IsTargetTag(hit.tag)) continue;

            GameObject root = hit.transform.root.gameObject;
            if (root == null) continue;
            if (processedRoots.Contains(root)) continue;

            processedRoots.Add(root);

            if (root.GetComponent<GhostFade>() != null)
                continue;

            TargetType targetType = GetTargetType(root);

            if (targetType == TargetType.Static)
            {
                if (enableStaticTargetInsideMask)
                {
                    MakeTargetVisibleInsideMask(root);
                }
                continue;
            }

            if (targetType == TargetType.Visible)
            {
                NotifyVisibleTarget(root);
                continue;
            }

            if (targetType == TargetType.None)
            {
                continue;
            }

            if (!CanCreateGhost(root))
                continue;

            CreateGhost(root);
            lastGhostTime[root] = Time.time;
        }
    }

    enum TargetType
    {
        None,
        Static,
        Dynamic,
        Visible
    }

    TargetType GetTargetType(GameObject target)
    {
        int layer = target.layer;
        string layerName = LayerMask.LayerToName(layer);

        if (visibleLayerList.Contains(layerName))
            return TargetType.Visible;

        if (staticLayerList.Contains(layerName))
            return TargetType.Static;

        if (dynamicLayerList.Contains(layerName))
            return TargetType.Dynamic;

        return TargetType.None;
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

    bool CanNotifyVisibleTarget(GameObject target)
    {
        if (!lastVisibleNotifyTime.ContainsKey(target))
            return true;

        return Time.time - lastVisibleNotifyTime[target] >= visibleNotifyCooldown;
    }

    void NotifyVisibleTarget(GameObject target)
    {
        if (!CanNotifyVisibleTarget(target))
            return;

        lastVisibleNotifyTime[target] = Time.time;

        MonoBehaviour[] behaviours = target.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour mb in behaviours)
        {
            if (mb == null) continue;

            if (mb is ISonarScannable scannable)
            {
                scannable.OnSonarScanned();
            }
        }
    }

    void MakeTargetVisibleInsideMask(GameObject target)
    {
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;

            sr.enabled = true;
            sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
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

    void CleanupNullVisibleNotifyTargets()
    {
        List<GameObject> removeList = new List<GameObject>();

        foreach (var kv in lastVisibleNotifyTime)
        {
            if (kv.Key == null)
                removeList.Add(kv.Key);
        }

        foreach (var key in removeList)
        {
            lastVisibleNotifyTime.Remove(key);
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
}