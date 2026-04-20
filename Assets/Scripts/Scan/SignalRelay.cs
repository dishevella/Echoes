using UnityEngine;
using System.Collections.Generic;

public class SignalRelay : MonoBehaviour, ISonarScannable
{
    [Header("Relay Parts")]
    public CircleCollider2D relayScanCollider;   // 现在只是辅助，不再拿它算初始半径
    public Transform relayVisualMask;
    public SpriteMask relaySpriteMask;

    [Header("Relay Pulse")]
    public float pulseDuration = 3f;
    public float visualStartScale = 0.2f;
    public float relayCooldown = 1f;

    [Header("Relay Chain")]
    public LayerMask relayLayer;

    private SonarGhostScanner scanner;

    private bool isPulsing = false;
    private float pulseTimer = 0f;
    private float pulseStartRadius = 0f;
    private float currentRadius = 0f;
    private float lastTriggerTime = -999f;

    private Vector3 originalMaskScale;
    private HashSet<SignalRelay> relaysTriggeredThisPulse = new HashSet<SignalRelay>();

    private void Awake()
    {
        scanner = GetComponent<SonarGhostScanner>();

        if (relayVisualMask != null)
        {
            originalMaskScale = relayVisualMask.localScale;
        }

        // 如果这个 collider 只是给计算/辅助用，可以关掉
        if (relayScanCollider != null)
        {
            relayScanCollider.enabled = false;
        }

        if (relayVisualMask != null)
        {
            relayVisualMask.gameObject.SetActive(false);
            relayVisualMask.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if (isPulsing)
        {
            UpdatePulse();
        }
    }

    public void OnSonarScanned()
    {
        ActivateRelay();
    }

    public void ActivateRelay()
    {
        if (isPulsing) return;
        if (Time.time - lastTriggerTime < relayCooldown) return;

        lastTriggerTime = Time.time;
        isPulsing = true;
        pulseTimer = 0f;

        relaysTriggeredThisPulse.Clear();

        if (relayVisualMask != null)
        {
            relayVisualMask.gameObject.SetActive(true);

            // 先把视觉层设到起始大小
            relayVisualMask.localScale = originalMaskScale * visualStartScale;

            // 再从视觉层反推出真实半径
            pulseStartRadius = GetVisualWorldRadius();
            currentRadius = pulseStartRadius;
        }
        else
        {
            // 没 visual 时兜底：还可以退回 collider 算法
            if (relayScanCollider != null)
            {
                pulseStartRadius = relayScanCollider.radius * Mathf.Abs(relayScanCollider.transform.lossyScale.x);
                currentRadius = pulseStartRadius;
            }
        }

        Debug.Log("[Relay] 开始脉冲, radius = " + pulseStartRadius);

        DoScanAndRelay(currentRadius);
    }

    void UpdatePulse()
    {
        pulseTimer += Time.deltaTime;
        float t = Mathf.Clamp01(pulseTimer / pulseDuration);

        currentRadius = Mathf.Lerp(pulseStartRadius, 0f, t);

        // 用“真实半径”反推视觉 scale
        SetVisualRadius(currentRadius);

        DoScanAndRelay(currentRadius);

        if (t >= 1f)
        {
            isPulsing = false;
            currentRadius = 0f;

            if (relayVisualMask != null)
            {
                relayVisualMask.localScale = Vector3.zero;
                relayVisualMask.gameObject.SetActive(false);
            }
        }
    }

    void DoScanAndRelay(float radius)
    {
        if (scanner != null)
        {
            scanner.ScanAt(transform.position, radius);
        }

        TriggerOtherRelays(radius);
    }

    void TriggerOtherRelays(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, relayLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            SignalRelay relay = hit.GetComponentInParent<SignalRelay>();
            if (relay == null) continue;
            if (relay == this) continue;
            if (relaysTriggeredThisPulse.Contains(relay)) continue;

            relaysTriggeredThisPulse.Add(relay);
            relay.ActivateRelay();
        }
    }

    float GetVisualWorldRadius()
    {
        if (relaySpriteMask == null || relaySpriteMask.sprite == null || relayVisualMask == null)
            return 0f;

        float spriteRadius = relaySpriteMask.sprite.bounds.extents.x;
        return spriteRadius * Mathf.Abs(relayVisualMask.lossyScale.x);
    }

    void SetVisualRadius(float targetRadius)
    {
        if (relaySpriteMask == null || relaySpriteMask.sprite == null || relayVisualMask == null)
            return;

        float spriteRadius = relaySpriteMask.sprite.bounds.extents.x;
        if (spriteRadius <= 0.0001f) return;

        float targetScale = targetRadius / spriteRadius;
        relayVisualMask.localScale = Vector3.one * targetScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentRadius > 0 ? currentRadius : pulseStartRadius);
    }
}