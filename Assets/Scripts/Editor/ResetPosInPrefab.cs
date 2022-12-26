using UnityEngine;
using UnityEditor;

public class ResetPosInPrefab : AssetPostprocessor
{
    void OnPostprocessPrefab(GameObject o)
    {
        o.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
