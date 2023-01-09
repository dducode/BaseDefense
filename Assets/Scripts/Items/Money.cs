using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Item
{
    ///<summary>Время, необходимое для проигрывания анимации сброса предмета на базу</summary>
    ///<remarks>Не может быть меньше 0</remarks>
    [SerializeField, Min(0), 
    Tooltip("Время, необходимое для проигрывания анимации сброса предмета на базу. Не может быть меньше 0")] 
    float collectionTime = 3;

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
