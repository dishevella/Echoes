using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

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
    public CheckPoint checkPoint;

    private SpriteRenderer[] spriteRenderers;
    private Collider2D[] colliders2D;
    private Rigidbody2D rb;

    private Color[] originalColors;
    private Coroutine fadeCoroutine;

    //public Action OnDead;

    private void Awake()
    {
        instance = this;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        colliders2D = GetComponentsInChildren<Collider2D>(true);
        rb = GetComponent<Rigidbody2D>();

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
        PlayAudio.instance.PlayDeath();

        if (MovementController.instance != null)
        {
            MovementController.instance.StopMove(reliveDelay);
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        foreach (Collider2D col in colliders2D)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        if (deathParticles != null)
        {
            ParticleSystem ps = Instantiate(deathParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, 3f);
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut());

        CancelInvoke(nameof(Relive));
        Invoke(nameof(Relive), reliveDelay);
    }

    private void KillAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(false);
            }
        }
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

        fadeCoroutine = null;
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        this.checkPoint = checkPoint;
    }

    private void Relive()
    {
        // 先停掉淡出协程，防止继续把角色拉透明
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        // 复活位置
        if (checkPoint != null)
        {
            transform.position = checkPoint.transform.position;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

            spriteRenderers[i].enabled = true;
            spriteRenderers[i].color = originalColors[i];
        }

        isDead = false;

        LevelReset.instance.ObjectRespawn();
    }

    public bool IsDead()
    {
        return isDead;
    }
}