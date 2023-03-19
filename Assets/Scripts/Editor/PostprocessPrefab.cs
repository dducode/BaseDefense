using UnityEditor;
using UnityEngine;

namespace Editor {

    public class PostprocessPrefab : AssetPostprocessor {

        private void OnPostprocessPrefab (GameObject o) {
            o.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (o.layer == 3 && o.GetComponent<Collider>() == null)
                o.AddComponent<BoxCollider>();
        }

    }

}