using Unity.VisualScripting;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{
    public bool Dead = false;
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
        if (Dead) return;

        if (other.CompareTag("Thorn"))
        {
            Debug.Log("玩家碰到刺了！");
            Dead = true;
            if (anim != null)
            {
                anim.SetBool("Dead", true);
            }
            PlayerHealth.instance.Die();

        }
    }
}