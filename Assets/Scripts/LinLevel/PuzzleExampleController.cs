using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public class PuzzleExample
{
    public GameObject puzzleObject;
    public int id;
}

public class PuzzleExampleController : MonoBehaviour
{
    public static PuzzleExampleController instance;

    public GameObject darkForm;
    public GameObject lightForm;
    
    public int[] numbers;
    private bool isPass = false;
    public bool isTrigger = false;

    public GameObject[] puzzleObjects;
    public PuzzleExample[] puzzleExamples;
    
    public GameObject door;
    private bool isOpen = false;
    public float speed = 3f;
    private Vector2 originPos;
    private Vector2 targetPos;

    public PuzzleDoor puzzleDoor;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        originPos = door.transform.position;
        targetPos = originPos - Vector2.up * 5f;
    }

    private void Update()
    {
        if (PuzzleTriggerController.instance.activatedTriggers.Count == puzzleObjects.Length)
        {
            bool pass = true;
            for (int i = 0; i < puzzleExamples.Length; i++)
            {
                if (PuzzleTriggerController.instance.activatedTriggers[i] != puzzleExamples[i].id + 1)
                {
                    pass = false;
                    break;
                }
            }
            if (pass && !isPass)
            {
                isPass = true;
                isOpen = true;
                puzzleDoor.gameObject.SetActive(false);
            }
        }

        if(isOpen)
        {
            door.transform.position = Vector2.Lerp(door.transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    public void Interact()
    {
        ShowPuzzleExample();
        puzzleDoor.gameObject.SetActive(true);
        puzzleDoor.Close();
        darkForm.SetActive(false);
        lightForm.SetActive(true);
    }

    private void ShowPuzzleExample()
    {
        if (!isTrigger)
        {            
            numbers = new int[] { 0, 1, 2, 3, 4, 5 };
            numbers = numbers.OrderBy(x => UnityEngine.Random.Range(0, 100)).ToArray();

            puzzleExamples = new PuzzleExample[puzzleObjects.Length];
            for (int i = 0; i < puzzleObjects.Length; i++)
            {
                puzzleExamples[i] = new PuzzleExample();
                puzzleExamples[i].puzzleObject = puzzleObjects[i];
                puzzleExamples[i].id = numbers[i];
            }

            StartCoroutine(ShowPuzzleExampleInOrder());
            CameraZoneScroller.instance.LockCameraForSeconds(5f);
        }
    }

    IEnumerator ShowPuzzleExampleInOrder()
    {
        for(int i=0;i<puzzleExamples.Length;i++)
        {
            GameObject darkForm = puzzleExamples[numbers[i]].puzzleObject.transform.Find("CrystalLampDark").gameObject;
            GameObject lightForm = puzzleExamples[numbers[i]].puzzleObject.transform.Find("CrystalLampLight").gameObject;

            darkForm.SetActive(false);
            lightForm.SetActive(true);
            yield return new WaitForSeconds(1f);
        }
        isTrigger = true;
    }

    public void HideExample()
    {
        foreach(var example in puzzleExamples)
        {
            GameObject darkForm = example.puzzleObject.GetComponent<PuzzleTrigger>().darkForm;
            GameObject lightForm = example.puzzleObject.GetComponent<PuzzleTrigger>().lightForm;

            darkForm.SetActive(true);
            lightForm.SetActive(false);
        }
        darkForm.SetActive(true);
        lightForm.SetActive(false);
        isTrigger = false;
    }
}
