using UnityEngine;

public class AutoCrusherTrigger : MonoBehaviour
{
    [Header("Reference")]
    public Crusher targetCrusher;

    [Header("Option")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnce) return;
        if (!collision.CompareTag("Player")) return;
        if (targetCrusher == null) return;

        targetCrusher.StartDrop();
        hasTriggered = true;
    }
}