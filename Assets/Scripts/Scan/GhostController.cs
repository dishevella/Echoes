using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [Header("Scan")]
    public float scanInterval = 3f;
    public List<string> targetTagList = new List<string>();

    [Header("Ghost Fade")]
    public float ghostLifetime = 10f;
    public float startAlpha = 0.7f;

    [Header("Disable On Ghost")]
    public bool disableRigidbody2D = true;
    public bool disableCollider2D = true;
    public bool disableAnimator = true;
    public List<string> disableComponentNames = new List<string>();

    private float timer;
    private CircleCollider2D circle;

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= scanInterval)
        {
            timer = 0f;
            ScanOnce();
        }
    }

    void ScanOnce()
    {
        // 用已有 CircleCollider2D 的半径
        float radius = circle.radius * transform.lossyScale.x;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            GameObject target = hit.transform.root.gameObject;

            if (!IsTargetTag(target.tag))
                continue;

            // ❗关键：不要复制 ghost 本身
            if (target.GetComponent<GhostFade>() != null)
                continue;

            CreateGhost(target);
        }
    }

    bool IsTargetTag(string tagName)
    {
        return targetTagList.Contains(tagName);
    }

    void CreateGhost(GameObject target)
    {
        GameObject ghost = Instantiate(target, target.transform.position, target.transform.rotation);
        ghost.transform.localScale = target.transform.localScale;

        // 避免再次被识别
        ghost.tag = "Untagged";

        DisableSelectedComponents(ghost);

        if (disableRigidbody2D)
        {
            foreach (var rb in ghost.GetComponentsInChildren<Rigidbody2D>())
            {
                rb.simulated = false;
            }
        }

        if (disableCollider2D)
        {
            foreach (var col in ghost.GetComponentsInChildren<Collider2D>())
            {
                col.enabled = false;
            }
        }

        if (disableAnimator)
        {
            foreach (var anim in ghost.GetComponentsInChildren<Animator>())
            {
                anim.enabled = false;
            }
        }

        var fade = ghost.AddComponent<GhostFade>();
        fade.lifetime = ghostLifetime;
        fade.startAlpha = startAlpha;
    }

    void DisableSelectedComponents(GameObject ghost)
    {
        foreach (var script in ghost.GetComponentsInChildren<MonoBehaviour>())
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
}