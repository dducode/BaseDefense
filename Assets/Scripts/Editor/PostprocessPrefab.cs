using UnityEngine;
using UnityEditor;

public class PostprocessPrefab : AssetPostprocessor
{
    void OnPostprocessPrefab(GameObject o)
    {
        o.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (o.layer == 3 && o.GetComponent<Collider>() == null)
            o.AddComponent<BoxCollider>();
    }
}
