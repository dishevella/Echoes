using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static PlayerHealth instance;

    [Header("Death")]
    public float deathY = -30f;
    public float reliveDelay = 1f;

    [Header("Death FX")]
    public ParticleSystem deathParticles;
    public float fadeDuration = 1f;

    private bool isDead = false;
    private CheckPoint checkPoint;

    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] colliders2D;
    private Rigidbody2D rb;
    private Animator anim;

    private Color[] originalColors;

    private void Awake()
    {
        instance = this;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        colliders2D = GetComponentsInChildren<Collider2D>(true);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                originalColors[i] = spriteRenderers[i].color;
            }
        }
    }

    private void Update()
    {
        if (transform.position.y < deathY && !isDead)
        {
            Die();
        }

        if (Input.GetKeyDown(KeyCode.S))
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

        // 播放死亡动画参数
        if (anim != null)
        {
            anim.SetBool("Dead", true);
        }

        // 停止玩家控制
        if (MovementController.instance != null)
        {
            MovementController.instance.StopMove(reliveDelay);
        }

        // 停止刚体
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // 关闭碰撞，防止重复受伤
        foreach (Collider2D col in colliders2D)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // 播放粒子
        if (deathParticles != null)
        {
            ParticleSystem ps = Instantiate(deathParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, 3f);
        }

        // 渐隐
        StartCoroutine(FadeOut());

        // 延迟复活
        Invoke(nameof(Relive), reliveDelay);
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] == null) continue;

                Color c = originalColors[i];
                c.a = alpha;
                spriteRenderers[i].color = c;
            }

            yield return null;
        }
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        this.checkPoint = checkPoint;
    }

    private void Relive()
    {
        // 复活位置
        if (checkPoint != null)
        {
            transform.position = checkPoint.transform.position;
        }

        // 恢复刚体
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        // 恢复碰撞
        foreach (Collider2D col in colliders2D)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        // 恢复显示
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;
            spriteRenderers[i].color = originalColors[i];
        }

        // 恢复动画状态
        if (anim != null)
        {
            anim.SetBool("Dead", false);
        }

        isDead = false;
    }

    public bool IsDead()
    {
        return isDead;
    }
}