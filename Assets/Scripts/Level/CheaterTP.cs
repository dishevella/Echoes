using System.Collections.Generic;
using UnityEngine;

public class CheaterTP : MonoBehaviour
{
    public List<GameObject> spawnPoints = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (spawnPoints.Count > 0 && spawnPoints[0] != null)
                transform.position = spawnPoints[0].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (spawnPoints.Count > 1 && spawnPoints[1] != null)
                transform.position = spawnPoints[1].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (spawnPoints.Count > 2 && spawnPoints[2] != null)
                transform.position = spawnPoints[2].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (spawnPoints.Count > 3 && spawnPoints[3] != null)
                transform.position = spawnPoints[3].transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (spawnPoints.Count > 4 && spawnPoints[4] != null)
                transform.position = spawnPoints[4].transform.position;
        }
    }
}