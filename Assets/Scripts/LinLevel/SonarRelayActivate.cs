using UnityEngine;

public class SonarRelayActivate : MonoBehaviour
{
    public GameObject sonarRelay;
    private Vector3 spawnPos;

    private void Start()
    {
        spawnPos = transform.position + Vector3.up * 0.55f;
    }

    public void Interact()
    {
        Instantiate(sonarRelay, spawnPos, Quaternion.identity);
        Destroy(gameObject);
    }
}
