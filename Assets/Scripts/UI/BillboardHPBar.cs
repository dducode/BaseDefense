using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHPBar : MonoBehaviour
{
    Transform mainCamera;
    Vector3 startPos;

    void Start()
    {
        mainCamera = Camera.main.gameObject.transform;
        startPos = mainCamera.position - transform.position;
    }
    void LateUpdate()
    {
        transform.rotation = mainCamera.rotation;
        transform.position = mainCamera.position - startPos;
    }
}
