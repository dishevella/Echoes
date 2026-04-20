using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PatrolEnemyController : MonoBehaviour
{
    [SerializeField] private PatrolRoute patrolRoute;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float reachDistance = 0.1f;

    private int currentIndex = 0;
    private int direction = 1;
    private bool facingRight = true;
    private EnemyState currentState = EnemyState.Patrol;

    public bool FacingRight => facingRight;
    public EnemyState CurrentState => currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == EnemyState.Dead) return;
        if (patrolRoute == null) return;
        MoveAlongRoute();
    }
    private void MoveAlongRoute()
    {
        Transform target = patrolRoute.Points[currentIndex];
        if (target == null) return;

        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.position;

        Vector2 nextPos = Vector2.MoveTowards(currentPos, targetPos, speed *Time.deltaTime);
        transform.position = nextPos;

        UpdateFacing(targetPos - currentPos);
        if (Vector2.Distance(nextPos,targetPos) <= reachDistance)
            AdvanceIndex();
    }
    private void UpdateFacing(Vector2 MoveDir)
    {
        if (MoveDir.x > 0.01f && !facingRight)
            Flip(true);
        if (MoveDir.x < -0.01f && facingRight)
            Flip(false);
    }
    private void Flip(bool faceRight)
    {
        facingRight = faceRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1f : -1f);
        transform.localScale = scale;
    }
    private void AdvanceIndex()
    {
        int lastIndex = patrolRoute.Points.Length - 1;
        if(patrolRoute.PingPong)
        {
            if (currentIndex == lastIndex) direction = -1;
            else if (currentIndex == 0) direction = 1;
            currentIndex += direction;
        }
        else
        {
            currentIndex++;
            if(currentIndex> lastIndex)
            {
                currentIndex = 0;
            }
        }
    }
    public void SetDead()
    {
        currentState = EnemyState.Dead;

    }
}
