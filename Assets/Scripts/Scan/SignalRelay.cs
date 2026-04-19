using UnityEngine;
using System.Collections.Generic;

public class SignalRelay : MonoBehaviour, ISonarScannable
{
    [Header("Relay Parts")]
    public CircleCollider2D relayScanCollider;
    public Transform relayVisualMask;

    [Header("Relay Pulse")]
    public float pulseDuration = 1.2f;
    public float visualStartScale = 1f;
    public float relayCooldown = 0.3f;

    [Header("Relay Chain")]
    public LayerMask relayLayer;

    private SonarGhostScanner scanner;

    private bool isPulsing = false;
    private float pulseTimer = 0f;
    private float pulseStartRadius = 0f;
    private float currentRadius = 0f;
    private float lastTriggerTime = -999f;

    private HashSet<SignalRelay> relaysTriggeredThisPulse = new HashSet<SignalRelay>();

    private void Awake()
    {
        scanner = GetComponent<SonarGhostScanner>();

        if (relayScanCollider != null)
        {
            pulseStartRadius = relayScanCollider.radius * Mathf.Abs(relayScanCollider.transform.localScale.x);
            relayScanCollider.enabled = false;
        }

        if (relayVisualMask != null)
        {
            relayVisualMask.localScale = Vector3.zero;
            relayVisualMask.gameObject.SetActive(false);
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
        currentRadius = pulseStartRadius;

        relaysTriggeredThisPulse.Clear();

        if (relayVisualMask != null)
        {
            relayVisualMask.gameObject.SetActive(true);
            relayVisualMask.localScale = Vector3.one * visualStartScale;
        }

        DoScanAndRelay(currentRadius);
    }

    void UpdatePulse()
    {
        pulseTimer += Time.deltaTime;
        float t = Mathf.Clamp01(pulseTimer / pulseDuration);

        currentRadius = Mathf.Lerp(pulseStartRadius, 0f, t);

        if (relayVisualMask != null)
        {
            float k = pulseStartRadius > 0.0001f ? currentRadius / pulseStartRadius : 0f;
            relayVisualMask.localScale = Vector3.one * (visualStartScale * k);
        }

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

    private void OnDrawGizmosSelected()
    {
        if (relayScanCollider != null)
        {
            Gizmos.color = Color.cyan;
            float r = relayScanCollider.radius * Mathf.Abs(relayScanCollider.transform.lossyScale.x);
            Gizmos.DrawWireSphere(transform.position, r);
        }
    }
}