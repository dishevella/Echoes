using UnityEngine;

public class SonarBrowser : MonoBehaviour
{
    [Header("Follow")]
    public Transform target; // 玩家

    [Header("Scale")]
    public Vector3 originalScale;
    public float sonarSpeed = 5f;
    public float repairSpeed = 5f;
    public float maxScale = 3f;
    private Vector3 targetScale;
    private float speed;
    public float sonarTime = 1f;

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
        if (target != null)
        {
            transform.position = target.position;
        }

        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            targetScale,
            speed * Time.deltaTime
        );

        if (MovementController.instance != null && MovementController.instance.IsMove())
        {
            timer = 0f;
            hasTriggered = false;
            isReturning = false;

            SetScale(1f, repairSpeed);
            return;
        }

        if (isReturning)
        {
            if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
            {
                isReturning = false;
            }
            return;
        }

        if (!hasTriggered)
        {
            SetScale(maxScale, sonarSpeed);
            hasTriggered = true;
        }

        timer += Time.deltaTime;

        if (timer >= sonarTime)
        {
            timer = 0f;
            hasTriggered = false;

            SetScale(1f, repairSpeed);
            isReturning = true;
        }
    }

    public void SetScale(float index, float s)
    {
        targetScale = originalScale * index;
        speed = s;
    }
}