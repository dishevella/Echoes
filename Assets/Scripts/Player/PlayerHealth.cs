using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public static PlayerHealth instance;

    private bool isDead = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(transform.position.y < -50f && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(DamageInfo info)
    {
        if (isDead) return;

        Die();
    }

    private void Die()
    {
        isDead = true;
        gameObject.SetActive(false);
    }
}
