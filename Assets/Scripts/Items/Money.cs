using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Item
{
    [SerializeField] float collectionTime;
    [SerializeField] ForceMode forceMode = ForceMode.Impulse;
    SphereCollider trigger;
    Rigidbody rb;

    void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public override void DestroyItem()
    {
        trigger.enabled = false;
        StartCoroutine(DestroyThis());
    }

    public override void Drop(Vector3 force, Vector3 torque = default)
    {
        trigger.enabled = true;
        rb.isKinematic = false;
        rb.AddForce(force, forceMode);
        rb.AddTorque(torque, forceMode);
    }

    public void Collect()
    {
        trigger.enabled = false;
        rb.isKinematic = true;
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(collectionTime);
        ObjectsPool<Money>.Push(this);
    }
}
