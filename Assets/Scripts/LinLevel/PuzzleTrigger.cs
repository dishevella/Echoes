using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public int triggerID;

    public GameObject darkForm;
    public GameObject lightForm;

    private void Start()
    {
        //darkForm.SetActive(false);
    }

    public void Interact()
    {
        if (!PuzzleExampleController.instance.isTrigger) return;
        darkForm.SetActive(false);
        lightForm.SetActive(true);
        PuzzleTriggerController.instance.OnTriggerActivate(triggerID);
    }

    public void Resume()
    {
        darkForm.SetActive(true);
        lightForm.SetActive(false);
    }
}
