using UnityEngine;

public class KeyCheckerController : MonoBehaviour
{
    public static KeyCheckerController instance;

    private int currentKey = 0;
    public MovingPlateform movingPlateform;

    private void Awake()
    {
        instance = this;
    }

    public void MatchKey()
    {
        currentKey++;
        if(currentKey==3)
        {
            Debug.Log("Success");
            movingPlateform.isStart = true;
        }
    }
}
