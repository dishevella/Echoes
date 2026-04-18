using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float lifetime = 1.5f;
    public float startAlpha = 0.5f;

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

        float t = timer / lifetime;
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