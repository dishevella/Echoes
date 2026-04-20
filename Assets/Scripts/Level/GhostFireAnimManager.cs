using UnityEngine;
using System.Collections.Generic;

public class GhostFireAnimManager : MonoBehaviour
{
    public static GhostFireAnimManager Instance;

    public Sprite[] frames;
    public float frameRate = 8f;

    private float timer;
    private int currentFrame;
    private readonly List<GhostFireTrapVisual> activeVisuals = new List<GhostFireTrapVisual>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;

            Sprite currentSprite = frames[currentFrame];

            for (int i = activeVisuals.Count - 1; i >= 0; i--)
            {
                if (activeVisuals[i] == null)
                {
                    activeVisuals.RemoveAt(i);
                    continue;
                }

                activeVisuals[i].RefreshSprite(currentSprite);
            }
        }
    }

    public Sprite GetCurrentSprite()
    {
        if (frames == null || frames.Length == 0) return null;
        return frames[currentFrame];
    }

    public void Register(GhostFireTrapVisual visual)
    {
        if (visual == null) return;
        if (!activeVisuals.Contains(visual))
            activeVisuals.Add(visual);
    }

    public void Unregister(GhostFireTrapVisual visual)
    {
        if (visual == null) return;
        activeVisuals.Remove(visual);
    }
}