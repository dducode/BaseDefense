using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{  
    public override void AddImpulse(Vector3 force)
    {
        trailRenderer.Clear();
        rb.AddForce(force);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            other.gameObject.GetComponent<EnemyCharacter>().Hit(other.relativeVelocity.magnitude * rb.mass * 100);
        ObjectsPool<Bullet>.Push(this);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
