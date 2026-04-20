using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {

        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable == null) return;
        DamageInfo info = new DamageInfo
        {
            hitPoint = collision.transform.position,
            SourcePosition = transform.position,
            hitDirection = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized
        };
        damageable.TakeDamage(info);
    }
}
