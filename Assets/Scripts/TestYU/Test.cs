using UnityEngine;

public class Test : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DamageInfo info = new DamageInfo
        {

        };
        if(Input.GetKeyDown(KeyCode.A ))
        {
            enemyHealth.TakeDamage(info);
        }
    }
}
