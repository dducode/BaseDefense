using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHealthPointsBar : MonoBehaviour
{
    [SerializeField] BillboardMode billboardMode;

    Transform mainCamera;
    Vector3 startPosition;

    void Start()
    {
        mainCamera = Camera.main.gameObject.transform;
        startPosition = mainCamera.position - transform.position;
    }
    void LateUpdate()
    {
        if (billboardMode == BillboardMode.Full)
            transform.SetPositionAndRotation(mainCamera.position - startPosition, mainCamera.rotation);
        else if (billboardMode == BillboardMode.OnlyRotation)
            transform.rotation = mainCamera.rotation;
        else
        {
            Debug.LogError($"Not implemented billboard mode {billboardMode}");
            this.enabled = false;
        }
    }

    public enum BillboardMode
    {
        Full, OnlyRotation
    }
}
