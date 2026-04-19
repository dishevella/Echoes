using UnityEngine;
using System.Collections;

public class SonarBrowser : MonoBehaviour
{
    [Header("Follow")]
    public Transform target; // ⭐ 玩家

    public Vector3 originalScale;
    public float sonarSpeed = 5f;
    public float repairSpeed;
    public float maxScale;
    private Vector3 targetScale;
    private float speed;
    public float sonarTime;

    private float timer;
    private bool hasTriggered = false;
    private bool isReturning = false;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // ⭐ 跟随玩家
        if (target != null)
        {
            transform.position = target.position;
        }

        // ⭐ 缩放逻辑（原样保留）
        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            targetScale,
            speed * Time.deltaTime
        );

        if (MovementController.instance.IsMove())
        {
            timer = 0;
            hasTriggered = false;
            isReturning = false;

            SetScale(1f, repairSpeed);
            return;
        }

        // 如果正在缩小 → 等它缩完
        if (isReturning)
        {
            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                isReturning = false;
            }
            return;
        }

        // 第一次触发 sonar
        if (!hasTriggered)
        {
            SetScale(maxScale, sonarSpeed);
            hasTriggered = true;
        }

        timer += Time.deltaTime;

        if (timer >= sonarTime)
        {
            timer = 0;
            hasTriggered = false;

            SetScale(1f, repairSpeed);

            // 进入缩小阶段
            isReturning = true;
        }
    }
    
    public void SetScale(float index, float s)
    {
        targetScale = originalScale * index;
        speed = s;
    }
}