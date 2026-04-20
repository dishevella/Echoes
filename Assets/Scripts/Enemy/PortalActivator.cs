using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    [Header("Portal")]
    public GameObject portalObject;

    [Header("Trigger State")]
    public bool trigger1Activated;
    public bool trigger2Activated;
    public bool trigger3Activated;

    private bool portalOpened = false;

    private void Start()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
    }

    public void ActivateTrigger(int triggerID)
    {
        switch (triggerID)
        {
            case 1:
                trigger1Activated = true;
                break;
            case 2:
                trigger2Activated = true;
                break;
            case 3:
                trigger3Activated = true;
                break;
        }

        CheckPortalOpen();
    }

    private void CheckPortalOpen()
    {
        if (portalOpened) return;

        if (trigger1Activated && trigger2Activated && trigger3Activated)
        {
            portalOpened = true;

            if (portalObject != null)
            {
                portalObject.SetActive(true);
            }

            Debug.Log("All 3 triggers activated. Portal opened.");
        }
    }
}