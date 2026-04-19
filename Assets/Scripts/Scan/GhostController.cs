using UnityEngine;

public class GhostController : MonoBehaviour
{
    private CircleCollider2D circle;
    private SonarGhostScanner scanner;

    void Awake()
    {
        circle = GetComponent<CircleCollider2D>();
        scanner = GetComponent<SonarGhostScanner>();
    }

    void Update()
    {
        if (circle == null || scanner == null) return;

        float radius = circle.radius * Mathf.Abs(transform.lossyScale.x);
        scanner.ScanAt(transform.position, radius);
    }

    private void OnDrawGizmosSelected()
    {
        CircleCollider2D c = GetComponent<CircleCollider2D>();
        if (c == null) return;

        Gizmos.color = Color.cyan;
        float radius = c.radius * Mathf.Abs(transform.lossyScale.x);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}