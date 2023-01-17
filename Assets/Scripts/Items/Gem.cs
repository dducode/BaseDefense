using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item
{
    public override void Destroy()
    {
        StartCoroutine(DestroyGem());
    }

    public override void Drop(Vector3 force, Vector3 torque = default)
    {
        enabled = true;
        transform.localScale = Vector3.one;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    IEnumerator DestroyGem()
    {
        enabled = false;
        yield return Collapse();
        ObjectsPool<Gem>.Push(this);
    }
}
