using UnityEngine;

public class KeyRotate : MonoBehaviour
{
    public float rotateSpeed = 100f;

    private void Update()
    {
        transform.Rotate(new Vector3(0f, rotateSpeed * Time.deltaTime, 0f));
    }
}
