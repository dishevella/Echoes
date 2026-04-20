using UnityEngine;

public class CameraZoneScroller : MonoBehaviour
{
    public static CameraZoneScroller instance;
    
    [Header("Target")]
    public Transform player;

    [Header("Trigger Zone")]
    [Range(0.05f, 0.45f)]
    public float sidePercent = 0.2f;

    [Header("Smooth Move")]
    public float moveDuration = 0.8f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("World Bounds")]
    public float minX = 0f;
    public float maxX = 50f;
    public float minY = -10f;
    public float maxY = 10f;

    [Header("Lock Camera")]
    public bool lockCamera = false;
    private float lockTimer = 0;
    public Transform lockedPosition;

    private Camera cam;

    private bool isMoving = false;
    private float moveTimer = 0f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Awake()
    {
        instance = this;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (player == null || cam == null) return;

        if (lockCamera)
        {
            Vector3 target = new Vector3(
                lockedPosition.position.x,
                lockedPosition.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                target,
                Time.deltaTime * 5f
            );

            lockTimer -= Time.deltaTime;
            if (lockTimer <= 0f)
            {
                lockCamera = false;
            }
            return;
        }

        if (isMoving)
        {
            UpdateCameraMove();
            return;
        }

        CheckAndStartMove();
    }

    void CheckAndStartMove()
    {
        Vector3 playerViewport = cam.WorldToViewportPoint(player.position);

        float playerX = player.position.x;
        float playerY = player.position.y;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float desiredX = transform.position.x;
        float desiredY = transform.position.y;

        // ===== X轴 =====
        if (playerViewport.x >= 1f - sidePercent)
        {
            float desiredViewportX = sidePercent;
            desiredX = playerX + halfWidth - 2f * halfWidth * desiredViewportX;
        }
        else if (playerViewport.x <= sidePercent)
        {
            float desiredViewportX = 1f - sidePercent;
            desiredX = playerX + halfWidth - 2f * halfWidth * desiredViewportX;
        }

        // ===== Y轴 =====
        if (playerViewport.y >= 1f - sidePercent)
        {
            float desiredViewportY = sidePercent;
            desiredY = playerY + halfHeight - 2f * halfHeight * desiredViewportY;
        }
        else if (playerViewport.y <= sidePercent)
        {
            float desiredViewportY = 1f - sidePercent;
            desiredY = playerY + halfHeight - 2f * halfHeight * desiredViewportY;
        }

        desiredX = Mathf.Clamp(desiredX, minX, maxX);
        desiredY = Mathf.Clamp(desiredY, minY, maxY);

        Vector3 newTarget = new Vector3(desiredX, desiredY, transform.position.z);

        if (Vector3.Distance(transform.position, newTarget) < 0.01f)
            return;

        startPos = transform.position;
        targetPos = newTarget;
        moveTimer = 0f;
        isMoving = true;
    }

    public void LockCameraForSeconds(float seconds)
    {
        lockCamera = true;
        lockTimer = seconds;
        isMoving = false;
    }

    void UpdateCameraMove()
    {
        moveTimer += Time.deltaTime;

        float t = Mathf.Clamp01(moveTimer / moveDuration);
        float curveT = moveCurve.Evaluate(t);

        Vector3 newPos = Vector3.Lerp(startPos, targetPos, curveT);
        transform.position = newPos;

        if (t >= 1f)
        {
            isMoving = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Camera drawCam = GetComponent<Camera>();
        if (drawCam == null || !drawCam.orthographic) return;

        float halfHeight = drawCam.orthographicSize;
        float halfWidth = halfHeight * drawCam.aspect;

        Vector3 camPos = transform.position;

        float leftX = camPos.x - halfWidth;
        float rightX = camPos.x + halfWidth;

        float bottomY = camPos.y - halfHeight;
        float topY = camPos.y + halfHeight;

        float leftTriggerX = Mathf.Lerp(leftX, rightX, sidePercent);
        float rightTriggerX = Mathf.Lerp(leftX, rightX, 1f - sidePercent);

        float bottomTriggerY = Mathf.Lerp(bottomY, topY, sidePercent);
        float topTriggerY = Mathf.Lerp(bottomY, topY, 1f - sidePercent);

        Gizmos.color = Color.yellow;

        // X线
        Gizmos.DrawLine(new Vector3(leftTriggerX, bottomY, 0), new Vector3(leftTriggerX, topY, 0));
        Gizmos.DrawLine(new Vector3(rightTriggerX, bottomY, 0), new Vector3(rightTriggerX, topY, 0));

        // Y线
        Gizmos.DrawLine(new Vector3(leftX, bottomTriggerY, 0), new Vector3(rightX, bottomTriggerY, 0));
        Gizmos.DrawLine(new Vector3(leftX, topTriggerY, 0), new Vector3(rightX, topTriggerY, 0));
    }
}