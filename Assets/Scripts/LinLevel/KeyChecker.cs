using UnityEngine;

public class KeyChecker : MonoBehaviour
{
    public PropSO matchedKeySO;
    public GameObject open;
    public GameObject close;

    public void Interact()
    {
        if(BagSystem.instance.GetSelectedProp()==matchedKeySO)
        {
            KeyCheckerController.instance.MatchKey();
            BagSystem.instance.UseProp();
            open.SetActive(true);
            close.SetActive(false);
            PlayAudio.instance.PlayOpenBox();
        }
    }
}
