using UnityEngine;

public class EnemyWakeTrigger : MonoBehaviour
{
    public WoodenEnemy[] targetEnemies;
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnce) return;
        if (!collision.CompareTag("Player")) return;

        for (int i = 0; i < targetEnemies.Length; i++)
        {
            if (targetEnemies[i] != null)
            {
                targetEnemies[i].StartChasing();
            }
        }

        hasTriggered = true;
    }
}