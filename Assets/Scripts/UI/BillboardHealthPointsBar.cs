using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardHealthPointsBar : MonoBehaviour
{
    Transform mainCamera;

    void Start() => mainCamera = Camera.main.gameObject.transform;
    void LateUpdate() => transform.rotation = mainCamera.rotation;
}
