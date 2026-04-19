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
        if(transform.position.y < -50f && !isDead)
        {
            Die();
        }
        if (Input.GetKeyDown(KeyCode.S)) Die();
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
        Invoke("Relive", 2f);
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        this.checkPoint = checkPoint;
    }

    private void Relive()
    {
        gameObject.SetActive(true);
        transform.position = checkPoint.transform.position;
    }
}
