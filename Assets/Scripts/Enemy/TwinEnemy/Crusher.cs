using System.Collections;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [Header("Move")]
    public Transform targetDownPoint;
    public float dropSpeed = 8f;
    public float returnSpeed = 4f;
    public float stayTimeAfterLand = 1f;

    [Header("Detect")]
    public Transform killCheckCenter;
    public Vector2 killCheckSize = new Vector2(2f, 1f);
    public LayerMask twinLayer;

    public GameObject Portal;

    private bool isDropping = false;
    private bool isReturning = false;
    private bool hasLanded = false;

    private Vector3 startPosition;


    private void Awake()
    {
        startPosition = transform.position;
        Portal.SetActive(false);
    }

    public void StartDrop()
    {
        if (isDropping || isReturning) return;
        isDropping = true;
        hasLanded = false;
    }

    private void Update()
    {
        if (isDropping && targetDownPoint != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetDownPoint.position,
                dropSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetDownPoint.position) <= 0.02f)
            {
                transform.position = targetDownPoint.position;
                isDropping = false;
                hasLanded = true;

                CheckTwinsAndKill();
                StartCoroutine(ReturnToStartAfterDelay());
            }
        }
        else if (isReturning)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                returnSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, startPosition) <= 0.02f)
            {
                transform.position = startPosition;
                isReturning = false;
                hasLanded = false;
            }
        }
    }

    private IEnumerator ReturnToStartAfterDelay()
    {
        yield return new WaitForSeconds(stayTimeAfterLand);
        isReturning = true;
    }

    private void CheckTwinsAndKill()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            killCheckCenter.position,
            killCheckSize,
            0f,
            twinLayer
        );

        Brother brother = null;
        Sister sister = null;

        foreach (Collider2D hit in hits)
        {
            if (brother == null)
            {
                brother = hit.GetComponentInParent<Brother>();
            }

            if (sister == null)
            {
                sister = hit.GetComponentInParent<Sister>();
            }
        }

        if (brother != null && sister != null)
        {
            brother.Die();
            sister.Die();
            Portal.SetActive(true);
        }
        else
        {
           
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (killCheckCenter != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(killCheckCenter.position, killCheckSize);
        }
    }
}