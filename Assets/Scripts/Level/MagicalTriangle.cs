using System.Globalization;
using UnityEditor.Callbacks;
using UnityEngine;

public class MagicalTriangle : MonoBehaviour
{
    public GameObject target;
    public enum ObjectMode{Drop, Spwan}
    public ObjectMode om;

    private void Awake()
    {
        if(om == ObjectMode.Spwan)
            target.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(om == ObjectMode.Drop)
            {
                Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
                if (rb!= null)
                {
                    Debug.Log("地刺掉落触发！");
                    rb.gravityScale = 1f;
                }
            }

            if(om == ObjectMode.Spwan)
            {
                target.SetActive(true);
                Debug.Log("地刺生成触发！");
            }
        }
    }

}