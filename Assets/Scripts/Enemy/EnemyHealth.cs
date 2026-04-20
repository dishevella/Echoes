using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PatrolEnemyController patrolEnemyController;
    private bool isDead = false;
    public bool IsDead => isDead;
    public event Action<EnemyHealth> OnDead;
    public void TakeDamage(DamageInfo info)
    {
        if (isDead) return;
        Debug.Log($"{gameObject.name}");
        Die();
    }
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if(patrolEnemyController != null)
        {
            patrolEnemyController.SetDead();
        }
        OnDead?.Invoke(this);
        gameObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
