using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public Crusher crusher;
    private bool hasTriggered = false;

    public void ActivateLever()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        if (crusher != null)
        {
            crusher.StartDrop();
        }
    }
}