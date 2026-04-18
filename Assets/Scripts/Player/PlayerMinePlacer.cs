using UnityEngine;

public class PlayerMinePlacer : MonoBehaviour
{
    private Transform placePos;

    public GameObject minePrefab;
    public float placeDelay = 2f;

    private void Start()
    {
        placePos = transform.Find("MinePlacePos");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && MovementController.instance.IsGrounded())
        {
            Debug.Log("111");
            MovementController.instance.StopMove(placeDelay);
            Invoke("PlaceMine", placeDelay);
        }
    }

    private void PlaceMine()
    {
        Instantiate(minePrefab, placePos.position, Quaternion.identity);
    }
}
