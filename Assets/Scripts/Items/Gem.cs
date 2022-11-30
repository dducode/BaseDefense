using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item
{
    [SerializeField] ForceMode forceMode = ForceMode.Impulse;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void DestroyItem()
    {
        Pools.Push(this);
        gameObject.SetActive(false);
    }

    public override void Drop(Vector3 force, Vector3 torque = default)
    {
        gameObject.SetActive(true);
        rb.AddForce(force, forceMode);
        rb.AddTorque(torque, forceMode);
    }
}
