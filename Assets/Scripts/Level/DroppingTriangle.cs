using UnityEngine;

public class DroppingTriangle : MonoBehaviour
{
    public Collider2D triggerCollider; // ⭐ 指定的 collider
    public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;  // 关闭重力
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 只在“指定 collider”触发时才生效
        if (collision == triggerCollider && collision.CompareTag("Player"))
        {
            Debug.Log("移动平台触发！");
            rb.gravityScale = 1f;  // 恢复重力
        }
    }
}