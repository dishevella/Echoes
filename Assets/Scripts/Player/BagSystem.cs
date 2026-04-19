using System.Collections.Generic;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public static BagSystem instance;

    public List<PropSO> propSOList = new List<PropSO>();
    public List<PropSO> exampleSOList = new List<PropSO>();

    private PropSO selectedPropSO;
    private int slot = 0;

    public List<GameObject> selectedSlotList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (propSOList.Count > 0)
        {
            selectedPropSO = propSOList[0];
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchSelectedPropSO();
            UpdateSelectedSlot();
        }

        if(Input.GetKeyDown(KeyCode.E) && MovementController.instance.IsGrounded())
        {
            UseProp();
            UpdateSelectedSlot();
        }
    }

    public void AddProp(PropSO propSO)
    {
        propSOList.Add(propSO);
        if (selectedPropSO == null)
        {
            slot = 0;
            selectedPropSO = propSOList[0];
            UpdateSelectedSlot();
        }
    }
    public void RemoveProp(PropSO propSO)
    {
        propSOList.Remove(propSO);
        if (propSOList.Count == 0)
        {
            selectedPropSO = null;
            slot = 0;
            UpdateSelectedSlot();
            return;
        }

        if (slot >= propSOList.Count)
        {
            slot = 0;
        }

        selectedPropSO = propSOList[slot];
    }

    private void SwitchSelectedPropSO()
    {
        if (propSOList.Count == 0) return;
        
        slot = (slot+1) % propSOList.Count;
        selectedPropSO = propSOList[slot];
    }

    public void UseProp()
    {
        if (selectedPropSO == null) return;
        if (!IsHaveProp(selectedPropSO)) return;

        switch(FindIndex())
        {
            case 0:
                PlayerMinePlacer.instance.PlaceMine();
                RemoveProp(selectedPropSO);
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }

    private int FindIndex()
    {
        for(int i=0;i<exampleSOList.Count;i++)
        {
            if (selectedPropSO == exampleSOList[i])
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsHaveProp(PropSO propSO)
    {
        if (propSOList.Contains(propSO)) 
            return true;
        else 
            return false;
    }

    private void UpdateSelectedSlot()
    {
        if (selectedSlotList.Count == 0) return;

        foreach(var mask in selectedSlotList)
        {
            mask.SetActive(false);
        }
        selectedSlotList[slot].SetActive(true);
    }
}
