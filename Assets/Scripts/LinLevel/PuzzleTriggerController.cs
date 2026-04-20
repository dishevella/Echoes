using System.Collections.Generic;
using UnityEngine;

public class PuzzleTriggerController : MonoBehaviour
{
    public static PuzzleTriggerController instance;
    
    public PuzzleTrigger[] puzzleTriggers;
    public List<int> activatedTriggers = new List<int>();

    public PuzzleDoor puzzleDoor;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(activatedTriggers.Count==puzzleTriggers.Length)
        {
            foreach(var trigger in puzzleTriggers)
            {
                trigger.Resume();
            }
            PuzzleExampleController.instance.HideExample();
            Invoke(nameof(Resume), 0.5f);
<<<<<<< HEAD
            puzzleDoor.Open();
=======
            puzzleDoor.Open();           
>>>>>>> origin/LinKejun5
            PuzzleExampleController.instance.HidePuzzleDoorAfterDelay(1f);
        }
    }

    public void OnTriggerActivate(int id)
    {
        if(!activatedTriggers.Contains(id))
        {
            activatedTriggers.Add(id);
        }
    }

    public void Resume()
    {
        activatedTriggers.Clear();
    }
}
