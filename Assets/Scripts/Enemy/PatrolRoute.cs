using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private bool pingPong = true;

    public Transform[] Points => points;
    public bool PingPong => pingPong;

    private void OnDrawGizmos()
    {
        if (points == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < points.Length; i++)
        {
            if (points == null) continue;
            Gizmos.DrawSphere(points[i].position, 0.2f);

            if (i < points.Length - 1 && points[i + 1] != null)
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
