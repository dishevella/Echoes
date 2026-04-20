using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 worldOffset = new Vector3(0, 1.2f, 0);

    private RectTransform rect;
    private Canvas canvas;
    private RectTransform canvasRect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 worldPos = target.position + worldOffset;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        rect.anchoredPosition = localPoint;   // 关键：不要用 localPosition
    }
}