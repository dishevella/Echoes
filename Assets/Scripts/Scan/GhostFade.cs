using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float lifetime = 10f;
    public float startAlpha = 0.7f;

    private float timer = 0f;
    private SpriteRenderer[] spriteRenderers;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        SetAlpha(startAlpha);
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / lifetime);
        float alpha = Mathf.Lerp(startAlpha, 0f, t);

        SetAlpha(alpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr == null) continue;

            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}