using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract void Drop(Vector3 force, Vector3 torque = default);
    public abstract void DestroyItem();
}
