using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 6;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridCenter = Vector2.zero;

    public GridNode[,] Grid { get; private set; }

    private Vector2 bottomLeftWorld;

    private void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        Grid = new GridNode[width, height];

        float gridWorldWidth = width * cellSize;
        float gridWorldHeight = height * cellSize;

        bottomLeftWorld = new Vector2(
            gridCenter.x - gridWorldWidth / 2f + cellSize / 2f,
            gridCenter.y - gridWorldHeight / 2f + cellSize / 2f
        );

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Grid[x, y] = new GridNode(x, y, true);
            }
        }
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public GridNode GetNode(int x, int y)
    {
        if (!IsInBounds(x, y)) return null;
        return Grid[x, y];
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - bottomLeftWorld.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - bottomLeftWorld.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(GridNode node)
    {
        float worldX = bottomLeftWorld.x + node.x * cellSize;
        float worldY = bottomLeftWorld.y + node.y * cellSize;
        return new Vector3(worldX, worldY, 0f);
    }

    private void OnDrawGizmos()
    {
        if (Grid == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridNode node = Grid[x, y];
                Vector3 worldPos = new Vector3(
                    bottomLeftWorld.x + x * cellSize,
                    bottomLeftWorld.y + y * cellSize,
                    0f
                );

                Gizmos.color = node.walkable ? Color.white : Color.yellow;
                Gizmos.DrawWireCube(worldPos, Vector3.one * cellSize * 0.9f);
            }
        }
    }
}