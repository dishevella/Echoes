using UnityEngine;

public class TwinManager : MonoBehaviour
{
    [Header("Scan Hold Time")]
    public float scannedHoldTime = 0.8f;

    private float BroScannedTimer = 0f;
    private float SisScannedTimer = 0f;

    public bool BroScanned => BroScannedTimer > 0f;
    public bool SisScanned => SisScannedTimer > 0f;

    public bool LinkActivated => BroScanned;

    private void Update()
    {
        if (BroScannedTimer > 0f)
            BroScannedTimer -= Time.deltaTime;

        if (SisScannedTimer > 0f)
            SisScannedTimer -= Time.deltaTime;
    }

    public void MarkBroScanned()
    {
        BroScannedTimer = scannedHoldTime;
    }

    public void MarkSisScanned()
    {
        SisScannedTimer = scannedHoldTime;
    }
}