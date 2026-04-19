using UnityEngine;

public class MovingPlateform : MonoBehaviour
{
    private Vector3 originPos;
    public float moveDistance = 6f;
    public float speed = 3f;
    public bool isStart = false;

    private void Start()
    {
        originPos = transform.position;
    }

    private void Update()
    {
        if(isStart)
        {
            float yOffset = Mathf.PingPong(Time.time * speed, moveDistance);
            transform.position = originPos + new Vector3(0f, yOffset, 0f);
        }
    }
}
