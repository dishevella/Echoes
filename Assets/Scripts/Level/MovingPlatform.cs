using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path")]
    public List<Transform> points = new List<Transform>();

    [Header("Movement")]
    public float duration = 2f;
    public bool isLoop = true;

    [Header("Trigger")]
    public Collider2D triggerCollider;     // 触发区域
    public TriggerButton triggerButton;    // 按钮

    private bool isActivated = false;

    private int currentIndex = 0;
    private int direction = 1;

    private float timer = 0f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        if (points.Count < 2) return;

        transform.position = points[0].position;
        SetNextTarget();
    }

    void Update()
    {
        // 🎯 检查是否被激活（任意条件成立）
        CheckActivation();

        if (!isActivated) return;

        timer += Time.deltaTime;

        float t = timer / duration;
        t = Mathf.SmoothStep(0f, 1f, t);

        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (timer >= duration)
        {
            timer = 0f;
            SetNextTarget();
        }
    }

    void CheckActivation()
    {
        if(triggerButton == null && triggerCollider == null)
        {
            isActivated = true;
            return;
        }

        // 按钮触发
        if (triggerButton != null && triggerButton.isTrigger)
        {
            isActivated = true;
            return;
        }

        // TriggerCollider 会在 OnTriggerEnter 里触发
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerCollider == null) return;

        // 必须是指定的触发器
        if (other == triggerCollider)
        {
            isActivated = true;
        }
    }

    void SetNextTarget()
    {
        startPos = transform.position;

        currentIndex += direction;

        if (currentIndex >= points.Count)
        {
            if (isLoop)
            {
                currentIndex = 0;
            }
            else
            {
                direction = -1;
                currentIndex = points.Count - 2;
            }
        }

        if (currentIndex < 0)
        {
            direction = 1;
            currentIndex = 1;
        }

        targetPos = points[currentIndex].position;
    }
}