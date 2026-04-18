using UnityEngine;

public class MineController : MonoBehaviour
{
    public float explosionRadius = 2f;
    public LayerMask enemyLayer;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius,enemyLayer);

        foreach(var hit in hits)
        {
            if(hit.TryGetComponent<IDamageable>(out IDamageable ID))
            {
                DamageInfo info = new DamageInfo();
                ID.TakeDamage(info);
            }
        }
        Destroy(gameObject);
    }
}
