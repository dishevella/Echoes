using UnityEngine;

public class KeyChecker : MonoBehaviour
{
    public PropSO matchedKeySO;

    public void Interact()
    {
        if(BagSystem.instance.GetSelectedProp()==matchedKeySO)
        {
            Debug.Log("Match");
            KeyCheckerController.instance.MatchKey();
            BagSystem.instance.UseProp();
        }
    }
}
