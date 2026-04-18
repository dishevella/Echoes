using UnityEngine;
using System.Collections;

public class SonarBrowser : MonoBehaviour
{
    public Vector3 originalScale;
    public float sonarSpeed = 5f;
    public float repairSpeed;
    public float maxScale;
    private Vector3 targetScale;
    private float speed;
    public float sonarTime;

    private float timer;
    private bool hasTriggered = false;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // 匀速缩放
        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            targetScale,
            speed * Time.deltaTime
        );

        if (MovementController.instance.IsMove())
        {
            timer = 0;
            hasTriggered = false;
            return;
        }

        // 第一次触发 sonar
        if (!hasTriggered)
        {
            SetScale(maxScale, sonarSpeed);
            hasTriggered = true;
        }

        // 计时
        timer += Time.deltaTime;

        if (timer >= sonarTime)
        {
            timer = 0;
            hasTriggered = false;

            SetScale(1f, repairSpeed);
        }
    }

    public void SetScale(float index, float s)
    {
        targetScale = originalScale * index;
        speed = s;
    }
}