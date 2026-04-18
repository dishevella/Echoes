using UnityEngine;

public class CameraZoneScroller : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Trigger Zone")]
    [Range(0.05f, 0.45f)]
    public float sidePercent = 0.2f;   // 左右边界百分比，0.2 = 20%

    [Header("Smooth Move")]
    public float moveDuration = 0.8f;  // 相机完成一次滑动的大致时长
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("World Bounds")]
    public float minX = 0f;
    public float maxX = 50f;

    private Camera cam;

    private bool isMoving = false;
    private float moveTimer = 0f;

    private float startX;
    private float targetX;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (player == null || cam == null) return;

        // 正在移动中：继续平滑运动
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

        // 玩家进右侧 20%
        if (playerViewport.x >= 1f - sidePercent)
        {
            StartMoveRight();
        }
        // 玩家进左侧 20%
        else if (playerViewport.x <= sidePercent)
        {
            StartMoveLeft();
        }
    }

    void StartMoveRight()
    {
        float playerX = player.position.x;
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;

        // 目标：让玩家最终落在左侧 20%
        float desiredViewportX = sidePercent;
        float desiredCameraX = playerX + cameraHalfWidth - 2f * cameraHalfWidth * desiredViewportX;

        desiredCameraX = Mathf.Clamp(desiredCameraX, minX, maxX);

        // 如果已经到头或几乎不用动，就不启动
        if (Mathf.Abs(transform.position.x - desiredCameraX) < 0.01f)
            return;

        startX = transform.position.x;
        targetX = desiredCameraX;
        moveTimer = 0f;
        isMoving = true;
    }

    void StartMoveLeft()
    {
        float playerX = player.position.x;
        float cameraHalfWidth = cam.orthographicSize * cam.aspect;

        // 目标：让玩家最终落在右侧 20%
        float desiredViewportX = 1f - sidePercent;
        float desiredCameraX = playerX + cameraHalfWidth - 2f * cameraHalfWidth * desiredViewportX;

        desiredCameraX = Mathf.Clamp(desiredCameraX, minX, maxX);

        // 如果已经到头或几乎不用动，就不启动
        if (Mathf.Abs(transform.position.x - desiredCameraX) < 0.01f)
            return;

        startX = transform.position.x;
        targetX = desiredCameraX;
        moveTimer = 0f;
        isMoving = true;
    }

    void UpdateCameraMove()
    {
        moveTimer += Time.deltaTime;

        float t = Mathf.Clamp01(moveTimer / moveDuration);
        float curveT = moveCurve.Evaluate(t);

        float newX = Mathf.Lerp(startX, targetX, curveT);

        Vector3 pos = transform.position;
        pos.x = newX;
        transform.position = pos;

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

        float leftTriggerX = Mathf.Lerp(leftX, rightX, sidePercent);
        float rightTriggerX = Mathf.Lerp(leftX, rightX, 1f - sidePercent);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(leftTriggerX, camPos.y - halfHeight, 0f),
            new Vector3(leftTriggerX, camPos.y + halfHeight, 0f)
        );

        Gizmos.DrawLine(
            new Vector3(rightTriggerX, camPos.y - halfHeight, 0f),
            new Vector3(rightTriggerX, camPos.y + halfHeight, 0f)
        );
    }
}