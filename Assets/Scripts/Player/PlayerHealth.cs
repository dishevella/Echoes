using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public static PlayerHealth instance;

    private bool isDead = false;
    private CheckPoint checkPoint;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(transform.position.y < -30f && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        if (isDead) return;

        Die();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // ❌ 不要立刻隐藏
        // gameObject.SetActive(false);

        // ⭐ 停止玩家控制（关键）
        MovementController.instance.StopMove(2f);

        // ⭐ 1秒后再复活
        Invoke(nameof(Relive), 1f);
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        this.checkPoint = checkPoint;
    }

    private void Relive()
    {
        gameObject.SetActive(true);
        // ⭐ 恢复动画状态
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("Dead", false);
        }
        isDead = false;
        transform.position = checkPoint.transform.position;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
