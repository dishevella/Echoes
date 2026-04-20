using UnityEngine;

public class TwinWakeTrigger : MonoBehaviour
{
    [Header("Targets")]
    public Brother targetBrother;
    public Sister targetSister;

    [Header("Option")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnce) return;
        if (!collision.CompareTag("Player")) return;

        if (targetBrother != null)
        {
            targetBrother.StartMoving();
        }

        if (targetSister != null)
        {
            targetSister.StartMoving();
        }

        hasTriggered = true;
    }
}