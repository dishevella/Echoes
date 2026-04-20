using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject door;

    private Vector2 originPos;
    private Vector2 targetPos;
    public float speed = 3f;

    private bool isOpen = false;
    private bool isClose = false;

    private Vector2 buttonOriginPos;
    private Vector2 buttonTargetPos;

    private void Start()
    {
        originPos = door.transform.position;
        targetPos = originPos + Vector2.up * 5f;

        buttonOriginPos = transform.position;
        buttonTargetPos = buttonOriginPos - Vector2.up * 0.3f;
    }

    private void Update()
    {
        if(isOpen)
        {
            door.transform.position = Vector2.Lerp(door.transform.position, targetPos, speed * Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, buttonTargetPos, speed * Time.deltaTime);
        }
        else if(isClose)
        {
            door.transform.position = Vector2.Lerp(door.transform.position, originPos, speed * Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, buttonOriginPos, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Moveable"))
        {
            isOpen = true;
            isClose = false;
            PlayAudio.instance.PlayStone();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Moveable"))
        {
            isOpen = false;
            isClose = true;
            PlayAudio.instance.PlayStone();
        }
    }

}
