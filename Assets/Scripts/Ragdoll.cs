using UnityEngine;
using System.Collections.Generic;

public class Ragdoll : MonoBehaviour
{
    Collider[] colliders;
    Rigidbody[] rigidbodies;
    Rigidbody mainRigidbody;

    bool _enabled;
    public new bool enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            base.enabled = _enabled;
            foreach (Collider collider in colliders)
                collider.enabled = _enabled;
            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.isKinematic = !_enabled;
        }
    }

    void Awake()
    {
        List<Collider> colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        foreach (Collider collider in colliders)
            if (collider is CharacterController)
            {
                colliders.Remove(collider);
                this.colliders = colliders.ToArray();
                break;
            }
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
            if (rigidbody.gameObject.name.ToLower().Contains("pelvis"))
            {
                mainRigidbody = rigidbody;
                break;
            }
        enabled = false;
    }

    public void AddImpulse(Vector3 force)
    {
        mainRigidbody.AddForce(force, ForceMode.VelocityChange);
    }
}
