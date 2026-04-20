using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [Header("Reference")]
    public PortalActivator portalActivator;

    [Header("Trigger Setting")]
    public int triggerID = 1;
    public bool triggerOnce = true;

    private bool hasTriggered = false;
    public GameObject fire;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnce) return;
        if (!collision.CompareTag("Player")) return;
        if (portalActivator == null) return;

        portalActivator.ActivateTrigger(triggerID);
        hasTriggered = true;
        fire.SetActive(true);
        Debug.Log("Trigger " + triggerID + " activated.");
    }
}