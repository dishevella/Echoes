using UnityEngine;

public class MineController : MonoBehaviour
{
    public int damage = 100;
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
                ID.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
