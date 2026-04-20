using System.Collections.Generic;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public static BagSystem instance;

    public List<PropSO> propSOList = new List<PropSO>();
    public List<PropSO> exampleSOList = new List<PropSO>();

    public PropSO selectedPropSO;

    // 这是“背包里的选中索引”，不是UI槽位索引
    private int selectedIndex = 0;

    public List<GameObject> selectedSlotList = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (propSOList.Count > 0)
        {
            selectedIndex = 0;
            selectedPropSO = propSOList[0];
        }

        UpdateSelectedSlot();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchSelectedPropSO();
            UpdateSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.E) && MovementController.instance.IsGrounded())
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
            selectedIndex = 0;
            selectedPropSO = propSOList[0];
        }

        switch (FindIndex(propSO))
        {
            case 0:
            case 1:
            case 2:
                KeyUIManager.instance.ShowKey(propSO);
                break;
        }

        UpdateSelectedSlot();
    }

    public void RemoveProp(PropSO propSO)
    {
        int removeIndex = propSOList.IndexOf(propSO);
        if (removeIndex == -1) return;

        propSOList.RemoveAt(removeIndex);

        if (propSOList.Count == 0)
        {
            selectedPropSO = null;
            selectedIndex = 0;
            UpdateSelectedSlot();
            return;
        }

        // 如果删掉的是前面的物品，索引要往前补
        if (removeIndex < selectedIndex)
        {
            selectedIndex--;
        }

        // 如果删掉的是当前选中的物品
        if (removeIndex == selectedIndex)
        {
            if (selectedIndex >= propSOList.Count)
            {
                selectedIndex = 0;
            }
        }

        // 防止越界
        if (selectedIndex < 0) selectedIndex = 0;
        if (selectedIndex >= propSOList.Count) selectedIndex = 0;

        selectedPropSO = propSOList[selectedIndex];
        UpdateSelectedSlot();
    }

    private void SwitchSelectedPropSO()
    {
        if (propSOList.Count == 0) return;

        selectedIndex = (selectedIndex + 1) % propSOList.Count;
        selectedPropSO = propSOList[selectedIndex];
    }

    public void UseProp()
    {
        if (selectedPropSO == null) return;
        if (!IsHaveProp(selectedPropSO)) return;

        switch (FindIndex(selectedPropSO))
        {
            case 0:
                KeyUIManager.instance.HideKey(selectedPropSO);
                RemoveProp(selectedPropSO);          
                break;
            case 1:
                KeyUIManager.instance.HideKey(selectedPropSO);
                RemoveProp(selectedPropSO);
                break;
            case 2:
                KeyUIManager.instance.HideKey(selectedPropSO);
                RemoveProp(selectedPropSO);
                break;
            case 3:
                PlayerMinePlacer.instance.PlaceMine();
                RemoveProp(selectedPropSO);
                break;
        }
    }

    private int FindIndex(PropSO propSO)
    {
        for (int i = 0; i < exampleSOList.Count; i++)
        {
            if (propSO == exampleSOList[i])
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsHaveProp(PropSO propSO)
    {
        return propSOList.Contains(propSO);
    }

    // 把道具映射到固定UI槽位
    private int GetUISlotIndex(PropSO propSO)
    {
        if (propSO == null) return -1;

        int typeIndex = FindIndex(propSO);

        // 你的三把钥匙固定槽位：0 / 1 / 2
        if (typeIndex >= 0 && typeIndex <= 2)
        {
            return typeIndex;
        }

        // 其他道具如果以后有固定槽，再继续加
        // 这里先不给默认槽，避免亮错
        return -1;
    }

    private void UpdateSelectedSlot()
    {
        if (selectedSlotList.Count == 0) return;

        foreach (var mask in selectedSlotList)
        {
            mask.SetActive(false);
        }

        int uiSlotIndex = GetUISlotIndex(selectedPropSO);

        if (uiSlotIndex >= 0 && uiSlotIndex < selectedSlotList.Count)
        {
            selectedSlotList[uiSlotIndex].SetActive(true);
        }
    }

    public PropSO GetSelectedProp()
    {
        return selectedPropSO;
    }
}