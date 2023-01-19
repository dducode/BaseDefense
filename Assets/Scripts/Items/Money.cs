using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Item
{
    ///<summary>Время, необходимое для проигрывания анимации сброса предмета на базу</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Время, необходимое для проигрывания анимации сброса предмета на базу. [0, infinity]")]
    [SerializeField, Min(0)] float collectionTime = 3;

    public override void Destroy()
    {
        StartCoroutine(DestroyMoney());
    }

    public override void Drop(Vector3 force, Vector3 torque = default)
    {
        enabled = true;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    IEnumerator DestroyMoney()
    {
        trigger.enabled = false;
        yield return new WaitForSeconds(collectionTime);
        enabled = false;
        yield return Collapse();
        ObjectsPool<Money>.Push(this);
        transform.localScale = Vector3.one;
    }
}
