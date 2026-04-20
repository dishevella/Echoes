using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset = new Vector3(0, 1.2f, 0);

    private RectTransform rect;
    private Canvas canvas;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 worldPos = target.position + worldOffset;

        // 世界 → 屏幕
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            worldPos
        );

        // 屏幕 → UI坐标（关键！防止分辨率错位）
        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rect.localPosition = localPoint;
    }
}