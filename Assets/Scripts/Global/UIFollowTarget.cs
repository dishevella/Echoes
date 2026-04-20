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
<<<<<<< HEAD
        canvasRect = canvas.GetComponent<RectTransform>();
=======
    }
    {
        if (target == null) return;

        Vector3 worldPos = target.position + worldOffset;

<<<<<<< HEAD
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);

        Vector2 localPoint;
=======
        // 世界 → 屏幕
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            Camera.main,
            worldPos
        );

        // 屏幕 → UI坐标（关键！防止分辨率错位）
        Vector2 localPoint;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

>>>>>>> origin/LinKejun5
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

<<<<<<< HEAD
        rect.anchoredPosition = localPoint;   // 关键：不要用 localPosition
=======
        rect.localPosition = localPoint;
>>>>>>> origin/LinKejun5
    }
}