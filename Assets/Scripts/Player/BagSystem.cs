using System.Collections.Generic;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public List<PropSO> propSOList = new List<PropSO>();

    private PropSO selectedPropSO;
    private int slot = 0;

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
        }
    }

    public void AddProp(PropSO propSO)
    {
        propSOList.Add(propSO);
        if (selectedPropSO == null)
        {
            slot = 0;
            selectedPropSO = propSOList[0];
        }
    }
    public void RemoveProp(PropSO propSO)
    {
        propSOList.Remove(propSO);
        if (propSOList.Count == 0)
        {
            selectedPropSO = null;
            slot = 0;
            return;
        }

        if (slot >= propSOList.Count)
        {
            slot = 0;
        }

        selectedPropSO = propSOList[slot];
    }

    public void SwitchSelectedPropSO()
    {
        if (propSOList.Count == 0) return;
        
        slot = (slot+1) % propSOList.Count;
        selectedPropSO = propSOList[slot];
    }
}
