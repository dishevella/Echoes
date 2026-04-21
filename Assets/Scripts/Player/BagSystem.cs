using System.Collections.Generic;
using UnityEngine;

public class BagSystem : MonoBehaviour
{
    public static BagSystem instance;

    public List<PropSO> propSOList = new List<PropSO>();
    public List<PropSO> exampleSOList = new List<PropSO>();

    public PropSO selectedPropSO;

    // ���ǡ��������ѡ��������������UI��λ����
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

        // ���ɾ�����ǰ�����Ʒ������Ҫ��ǰ��
        if (removeIndex < selectedIndex)
        {
            selectedIndex--;
        }

        // ���ɾ����ǵ�ǰѡ�е���Ʒ
        if (removeIndex == selectedIndex)
        {
            if (selectedIndex >= propSOList.Count)
            {
                selectedIndex = 0;
            }
        }

        // ��ֹԽ��
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

    // �ѵ���ӳ�䵽�̶�UI��λ
    private int GetUISlotIndex(PropSO propSO)
    {
        if (propSO == null) return -1;

        int typeIndex = FindIndex(propSO);

        // �������Կ�׹̶���λ��0 / 1 / 2
        if (typeIndex >= 0 && typeIndex <= 2)
        {
            return typeIndex;
        }

        // ������������Ժ��й̶��ۣ��ټ�����
        // �����Ȳ���Ĭ�ϲۣ���������
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