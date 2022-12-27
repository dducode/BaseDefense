using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();
    
    public void Shot(Vector3 force)
    {
        rb.AddForce(force);
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            other.gameObject.GetComponent<EnemyCharacter>().GetDamage(other.relativeVelocity.magnitude * rb.mass * 100);
        ObjectsPool<Bullet>.Push(this);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
