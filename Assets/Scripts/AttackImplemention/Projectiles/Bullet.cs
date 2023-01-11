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
        if (other.gameObject.GetComponent<EnemyCharacter>() is EnemyCharacter enemy)
            enemy.Hit(other.relativeVelocity.magnitude * rb.mass * 100);
        ObjectsPool<Bullet>.Push(this);
        rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
    }
}
