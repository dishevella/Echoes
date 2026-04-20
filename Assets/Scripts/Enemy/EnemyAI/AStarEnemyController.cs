using System.Collections.Generic;
using UnityEngine;

public class AStarEnemyController : MonoBehaviour
{
    [SerializeField] private AStarPathfinder pathfinder;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float repathInterval = 0.5f;

    private List<GridNode> currentPath;
    private int currentPathIndex;
    private float repathTimer;
    private bool facingRight = true;

    private void Update()
    {
        if (player == null) return;

        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            RecalculatePath();
        }

        MoveAlongPath();
    }

    private void RecalculatePath()
    {
        Vector2Int startGrid = gridManager.WorldToGrid(transform.position);
        Vector2Int targetGrid = gridManager.WorldToGrid(player.position);

        currentPath = pathfinder.FindPath(startGrid, targetGrid);
        currentPathIndex = 0;
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        if (currentPathIndex >= currentPath.Count) return;

        Vector3 targetPos = gridManager.GridToWorld(currentPath[currentPathIndex]);
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 moveDir = targetPos - transform.position;

        transform.position = nextPos;

        UpdateFacing(moveDir);

        if (Vector3.Distance(transform.position, targetPos) <= 0.05f)
        {
            currentPathIndex++;
        }
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        if (moveDir.x > 0.01f && !facingRight)
        {
            Flip(true);
        }
        else if (moveDir.x < -0.01f && facingRight)
        {
            Flip(false);
        }
    }

    private void Flip(bool faceRight)
    {
        facingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        transform.localScale = scale;
    }
}