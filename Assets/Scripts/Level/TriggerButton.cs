using UnityEngine;

public class TriggerButton : MonoBehaviour
{
    public bool isTrigger = false;

    // 你可以自己写触发逻辑，比如踩上去
    public void Activate()
    {
        isTrigger = true;
    }
}