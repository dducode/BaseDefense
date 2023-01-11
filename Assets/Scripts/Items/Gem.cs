using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item
{
    public override void DestroyItem()
    {
        ObjectsPool<Gem>.Push(this);
    }

    public override void Drop(Vector3 force, Vector3 torque = default)
    {
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }
}
