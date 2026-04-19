using Unity.VisualScripting;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        // 自动找父物体的 Animator（防止挂在子物体上找不到）
        if (anim == null)
        {
            anim = GetComponentInParent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerHealth.instance.IsDead()) return;

        if (other.CompareTag("Thorn"))
        {
            Debug.Log("玩家碰到刺了！");
            if (anim != null)
            {
                anim.SetBool("Dead", true);
            }
            PlayerHealth.instance.Die();

        }
    }
}