using UnityEngine;

public class SisterWakeTrigger : MonoBehaviour
{
    public Sister targetSister;
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnce) return;
        if (!collision.CompareTag("Player")) return;
        if (targetSister == null) return;

        targetSister.StartMoving();
        hasTriggered = true;
    }
}