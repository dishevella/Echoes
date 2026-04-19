using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    private Vector2 originPos;
    private Vector2 targetPos;
    public float speed = 15f;

    public bool isOpen = false;
    public bool isClose = false;

    private void Start()
    {
        originPos = transform.position;
        targetPos = originPos - Vector2.up * 5f;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isClose)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos, speed * Time.deltaTime);
        }
        else if (isOpen)
        {
            transform.position = Vector2.Lerp(transform.position, originPos, speed * Time.deltaTime);
        }
    }

    public void Open()
    {
        isOpen = true;
        isClose = false;
    }
    public void Close()
    {
        isOpen = false;
        isClose = true;
    }
}
