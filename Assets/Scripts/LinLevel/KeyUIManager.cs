using UnityEngine;
using UnityEngine.UI;

public class KeyUIManager : MonoBehaviour
{
    public static KeyUIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject[] keys;
    public PropSO[] keySO;

    public void ShowKey(PropSO propSO)
    {
        int index = 0;
        for(index=0;index<keys.Length;index++)
        {
            if (keySO[index]==propSO)
            {
                break;
            }
        }
        keys[index].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void HideKey(PropSO propSO)
    {
        int index = 0;
        for (index = 0; index < keys.Length; index++)
        {
            if (keySO[index] == propSO)
            {
                break;
            }
        }
        keys[index].GetComponent<Image>().color = new Color32(255, 255, 255, 50);
    }
}
