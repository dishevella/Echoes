using UnityEngine;

public class PlayerMinePlacer : MonoBehaviour
{
    public static PlayerMinePlacer instance;
    
    private Transform placePos;

    public PropSO mineSO;
    public float placeDelay = 2f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        placePos = transform.Find("MinePlacePos");
    }

    public void PlaceMine()
    {
        MovementController.instance.StopMove(placeDelay);
        Invoke(nameof(SpawnMine), placeDelay);
    }

    private void SpawnMine()
    {
        Instantiate(mineSO.prefab, placePos.position, Quaternion.identity);
    }
}
