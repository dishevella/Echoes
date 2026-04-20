using UnityEngine;

public class EnemyIgnoreWallByPlayerState : MonoBehaviour
{
    [Header("Reference")]
    public Collider2D targetWall;
    public Transform player;
    public LayerMask wallLayer;

    private Collider2D selfCollider;
    private Collider2D playerCollider;

    private bool isIgnoring = false;

    private void Awake()
    {
        selfCollider = GetComponent<Collider2D>();

        if (player != null)
        {
            playerCollider = player.GetComponent<Collider2D>();
        }
    }

    private void Update()
    {
        if (selfCollider == null || targetWall == null || playerCollider == null) return;

        bool playerOnWall = IsPlayerOnWall();

        if (playerOnWall)
        {
            SetIgnore(false); // 玩家在上面，不忽略
        }
        else
        {
            SetIgnore(true);  // 玩家不在上面，忽略
        }
    }

    private bool IsPlayerOnWall()
    {
        Bounds playerBounds = playerCollider.bounds;

        Vector2 checkOrigin = new Vector2(playerBounds.center.x, playerBounds.min.y);
        float checkDistance = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            checkOrigin,
            Vector2.down,
            checkDistance,
            wallLayer
        );

        if (hit.collider == null) return false;

        return hit.collider == targetWall;
    }

    private void SetIgnore(bool ignore)
    {
        if (isIgnoring == ignore) return;

        Physics2D.IgnoreCollision(selfCollider, targetWall, ignore);
        isIgnoring = ignore;
    }
}