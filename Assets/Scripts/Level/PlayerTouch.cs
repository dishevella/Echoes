using UnityEngine;
public class PlayerTouch : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerHealth.instance.IsDead()) return;

        if (other.CompareTag("Thorn"))
        {
            Debug.Log("玩家碰到刺了！");
            PlayerHealth.instance.Die();
        }

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("玩家碰到怪了！");
            PlayerHealth.instance.Die();
        }
    }
}