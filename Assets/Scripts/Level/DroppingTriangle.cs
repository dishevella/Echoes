using UnityEngine;

public class DroppingTriangleTrigger : MonoBehaviour
{
    public Rigidbody2D targetRb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("移动平台触发！");
            targetRb.gravityScale = 1f;
        }
    }
}